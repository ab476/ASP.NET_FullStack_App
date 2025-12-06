using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Features.DatabaseConfiguration.Provider;

public sealed class DatabaseConfigurationProvider(
    IDbContextFactory<ConfigurationDbContext> dbFactory,
    ILogger<DatabaseConfigurationProvider> logger) : IConfigurationProvider, IDisposable
{
    private readonly IDbContextFactory<ConfigurationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
    private readonly ILogger<DatabaseConfigurationProvider> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    // The dictionary instance will be atomically swapped on reloads.
    private ConcurrentDictionary<string, string?> _store = new(StringComparer.Ordinal);
    private readonly Lock _reloadLock = new();

    // Change token support
    private CancellationTokenSource _reloadTokenSource = new();

    // Track last seen (timestamp, key) pair to support deterministic delta loads.
    // Start at MinValue so first delta grabs everything.
    private DateTimeOffset _lastTimestamp = DateTimeOffset.MinValue;
    private string _lastKey = string.Empty;

    private bool _disposed;

    // Configuration for retries and paging
    private readonly int _maxRetries = 3;
    private readonly TimeSpan _baseRetryDelay = TimeSpan.FromSeconds(1);

    // ----------------------------
    // IConfigurationProvider API
    // ----------------------------

    public bool TryGet(string key, out string? value) =>
        _store.TryGetValue(key, out value);

    public void Set(string key, string? value)
    {
        ArgumentNullException.ThrowIfNull(key);

        if(_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Manual config set: {Key}.", key);

        // Update current store atomically (ConcurrentDictionary is safe for calls)
        _store[key] = value;

        // Notify consumers
        TriggerReload();
    }

    public IChangeToken GetReloadToken() =>
        new CancellationChangeToken(_reloadTokenSource.Token);

    public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
    {
        var prefix = string.IsNullOrEmpty(parentPath) ? string.Empty : parentPath + ":";

        return _store.Keys
            .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(k => k[prefix.Length..])
            .Concat(earlierKeys)
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Synchronous full load required by IConfigurationProvider.
    /// Uses synchronous EF Core queries to avoid deadlock in sync contexts.
    /// </summary>
    public void Load()
    {
        // We intentionally call the synchronous path here because IConfigurationProvider.Load is sync.
        // The async variants (LoadAsync / CheckAndReloadIfNeededAsync) are preferred in hosted scenarios.
        _logger.LogInformation("ConfigurationProvider: performing synchronous full load.");

        // Run a sync DB query. This uses the sync DbContext creation API.
        using var db = _dbFactory.CreateDbContext();

        // Read all entries in a deterministic order
        List<ConfigurationEntry> entries;
        try
        {
            entries = [.. db.Configurations
                .OrderBy(e => e.LastUpdated)
                .ThenBy(e => e.Key)
                .AsNoTracking()]; // sync
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Full synchronous configuration load failed.");
            throw;
        }

        // Build new store and atomically swap
        var newStore = new ConcurrentDictionary<string, string?>(StringComparer.Ordinal);
        DateTimeOffset maxTimestamp = _lastTimestamp;
        string maxKey = _lastKey;

        foreach (var e in entries)
        {
            newStore[e.Key] = e.Value;

            if (e.LastUpdated > maxTimestamp ||
               (e.LastUpdated == maxTimestamp && string.CompareOrdinal(e.Key, maxKey) > 0))
            {
                maxTimestamp = e.LastUpdated;
                maxKey = e.Key;
            }
        }

        // If no entries found, ensure we have at least the current time to avoid infinite reprocessing.
        if (entries.Count == 0 && _lastTimestamp == DateTimeOffset.MinValue)
        {
            maxTimestamp = DateTimeOffset.UtcNow;
            maxKey = string.Empty;
        }

        Interlocked.Exchange(ref _store, newStore);
        _lastTimestamp = maxTimestamp;
        _lastKey = maxKey;

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Synchronous full load complete. Loaded {Count} entries.", newStore.Count);
    }

    // ----------------------------
    // Async API - preferred for hosted polling
    // ----------------------------

    /// <summary>
    /// Perform a full asynchronous load (safe, atomic swap).
    /// </summary>
    public async Task LoadAsync(CancellationToken cancellation = default)
    {
        _logger.LogInformation("ConfigurationProvider: performing async full load.");

        await ExecuteWithRetriesAsync(async ct =>
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

            // Read everything ordered by (LastUpdated, Key)
            var entries = await db.Configurations
                .AsNoTracking()
                .OrderBy(e => e.LastUpdated)
                .ThenBy(e => e.Key)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            var newStore = new ConcurrentDictionary<string, string?>(StringComparer.Ordinal);

            DateTimeOffset maxTimestamp = _lastTimestamp;
            string maxKey = _lastKey;

            foreach (var e in entries)
            {
                newStore[e.Key] = e.Value;

                if (e.LastUpdated > maxTimestamp ||
                   (e.LastUpdated == maxTimestamp && string.CompareOrdinal(e.Key, maxKey) > 0))
                {
                    maxTimestamp = e.LastUpdated;
                    maxKey = e.Key;
                }
            }

            if (entries.Count == 0 && _lastTimestamp == DateTimeOffset.MinValue)
            {
                maxTimestamp = DateTimeOffset.UtcNow;
                maxKey = string.Empty;
            }

            // Atomically swap the store so readers see a consistent snapshot
            Interlocked.Exchange(ref _store, newStore);
            _lastTimestamp = maxTimestamp;
            _lastKey = maxKey;

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Async full load complete. Loaded {Count} entries.", newStore.Count);

        }, cancellation).ConfigureAwait(false);
    }

    /// <summary>
    /// Check database for delta updates and apply them to an atomic copy of the store.
    /// Uses (LastUpdated >= lastTimestamp) fetch and then filters in-memory to handle tie-breaker.
    /// </summary>
    public async Task CheckForUpdatesAsync(CancellationToken cancellation = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Checking for configuration updates since ({Timestamp}, {Key}).", _lastTimestamp, _lastKey);

        await ExecuteWithRetriesAsync(async ct =>
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

            // Get all rows with LastUpdated >= lastTimestamp (may include some that we already processed for tie-break).
            // We order so we can determine the new maximum (timestamp, key).
            var rows = await db.Configurations
                .AsNoTracking()
                .Where(e => e.LastUpdated >= _lastTimestamp)
                .OrderBy(e => e.LastUpdated)
                .ThenBy(e => e.Key)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            if (rows.Count == 0)
            {
                _logger.LogDebug("No updated configuration rows found.");
                return;
            }

            // Build a copy of the current store (shallow copy) so we apply delta changes atomically.
            var working = new ConcurrentDictionary<string, string?>(_store, StringComparer.Ordinal);

            DateTimeOffset maxTimestamp = _lastTimestamp;
            string maxKey = _lastKey;

            int applied = 0;

            foreach (var e in rows)
            {
                // Determine whether this row is new w.r.t (lastTimestamp, lastKey)
                var isNew =
                    (e.LastUpdated > _lastTimestamp) ||
                    (e.LastUpdated == _lastTimestamp && string.CompareOrdinal(e.Key, _lastKey) > 0);

                if (!isNew)
                    continue;

                working[e.Key] = e.Value;
                applied++;

                if (e.LastUpdated > maxTimestamp ||
                   (e.LastUpdated == maxTimestamp && string.CompareOrdinal(e.Key, maxKey) > 0))
                {
                    maxTimestamp = e.LastUpdated;
                    maxKey = e.Key;
                }
            }

            if (applied == 0)
            {
                _logger.LogDebug("Delta query returned rows, but none were newer than current position.");
                return;
            }

            // Atomically swap the store
            Interlocked.Exchange(ref _store, working);
            _lastTimestamp = maxTimestamp;
            _lastKey = maxKey;

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Applied {Applied} configuration updates. New position: ({Timestamp}, {Key})",
                applied, _lastTimestamp, _lastKey);

        }, cancellation).ConfigureAwait(false);
    }

    /// <summary>
    /// Convenience method: check for updates and if any were applied, trigger a reload token.
    /// </summary>
    public async Task CheckAndReloadIfNeededAsync(CancellationToken cancellation = default)
    {
        var beforeTimestamp = _lastTimestamp;
        var beforeKey = _lastKey;

        await CheckForUpdatesAsync(cancellation).ConfigureAwait(false);

        // If either part advanced, fire a change token.
        if (_lastTimestamp > beforeTimestamp || (_lastTimestamp == beforeTimestamp && string.CompareOrdinal(_lastKey, beforeKey) > 0))
        {
            _logger.LogInformation("Configuration changes detected; issuing reload token.");
            TriggerReload();
        }
    }

    // ----------------------------
    // Change token helpers
    // ----------------------------

    public void TriggerReload()
    {
        // Use lock & Interlocked.Exchange pattern to be safe
        lock (_reloadLock)
        {
            _logger.LogDebug("Triggering config reload token.");

            var old = Interlocked.Exchange(ref _reloadTokenSource, new CancellationTokenSource());
            try
            {
                old.Cancel();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Exception while cancelling old reload token (ignored).");
            }
            finally
            {
                old.Dispose();
            }
        }
    }

    // ----------------------------
    // Dispose
    // ----------------------------

    public void Dispose()
    {
        if (_disposed) return;

        _logger.LogDebug("Disposing DatabaseConfigurationProvider.");
        try
        {
            var cts = Interlocked.Exchange(ref _reloadTokenSource, new CancellationTokenSource());
            try { cts.Cancel(); } catch { }
            cts.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Exception while disposing provider reload token.");
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    // ----------------------------
    // Helpers
    // ----------------------------

    /// <summary>
    /// Execute an async action with simple retry/backoff for transient DB failures.
    /// </summary>
    private async Task ExecuteWithRetriesAsync(Func<CancellationToken, Task> action, CancellationToken cancellation = default)
    {
        var attempt = 0;
        var delay = _baseRetryDelay;

        while (true)
        {
            cancellation.ThrowIfCancellationRequested();
            try
            {
                await action(cancellation).ConfigureAwait(false);
                return;
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                // Propagate cancellation
                throw;
            }
            catch (Exception ex) when (IsTransient(ex) && attempt < _maxRetries)
            {
                attempt++;

                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning(ex, "Transient DB error while performing configuration operation (attempt {Attempt}). Retrying in {Delay}.", attempt, delay);
                
                try
                {
                    await Task.Delay(delay, cancellation).ConfigureAwait(false);
                }
                catch (OperationCanceledException) { throw; }

                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2); // exponential backoff
                continue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Non-transient error while performing configuration operation.");
                throw;
            }
        }
    }

    /// <summary>
    /// Basic heuristic for transient exceptions. You can swap this for Polly or a better detector.
    /// </summary>
    private static bool IsTransient(Exception ex)
    {
        // Very small heuristic:
        // - EF Core DbUpdateException/DbException may be wrapped; inspect inner exceptions.
        // - Network/timeout exceptions should be considered transient.
        // For production, integrate with a proper transient fault handling library (Polly).
        var baseEx = ex;
        while (baseEx.InnerException != null) baseEx = baseEx.InnerException;

        var typeName = baseEx.GetType().Name;

        return typeName.Contains("Timeout", StringComparison.OrdinalIgnoreCase)
            || typeName.Contains("Transient", StringComparison.OrdinalIgnoreCase)
            || typeName.Contains("Sql", StringComparison.OrdinalIgnoreCase)
            || typeName.Contains("MySql", StringComparison.OrdinalIgnoreCase)
            || typeName.Contains("Npgsql", StringComparison.OrdinalIgnoreCase)
            || typeName.Contains("DbException", StringComparison.OrdinalIgnoreCase);
    }
}
