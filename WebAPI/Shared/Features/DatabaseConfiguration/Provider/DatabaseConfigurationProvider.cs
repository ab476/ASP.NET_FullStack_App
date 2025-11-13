using Microsoft.Extensions.Primitives;

namespace Common.Features.DatabaseConfiguration.Provider;

public sealed class DatabaseConfigurationProvider(
    IDbContextFactory<ConfigurationDbContext> dbFactory,
    ILogger<DatabaseConfigurationProvider> _logger) : IConfigurationProvider, IDisposable
{
    private readonly IDbContextFactory<ConfigurationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
    private readonly Dictionary<string, string?> _store = new(StringComparer.Ordinal);
    private readonly object _reloadLock = new();
    private CancellationTokenSource _reloadTokenSource = new();
    private DateTimeOffset _lastChecked = DateTimeOffset.MinValue;
    private bool _disposed;

    // ---------------------------------------------------------
    // IConfigurationProvider Implementation
    // ---------------------------------------------------------

    public bool TryGet(string key, out string? value) =>
        _store.TryGetValue(key, out value);

    public void Set(string key, string? value)
    {
        ArgumentNullException.ThrowIfNull(key);

        _logger.LogDebug("Manual config set: {Key} updated.", key);

        _store[key] = value;
        TriggerReload();
    }

    public IChangeToken GetReloadToken() =>
        new CancellationChangeToken(_reloadTokenSource.Token);

    public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
    {
        var prefix = string.IsNullOrEmpty(parentPath) ? "" : parentPath + ":";

        return _store.Keys
            .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(k => k.Substring(prefix.Length))
            .Concat(earlierKeys)
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------
    // Initial Load (Full Load)
    // ---------------------------------------------------------

    public void Load()
    {
        _logger.LogInformation("Loading configuration entries from database (full load).");

        using var db = _dbFactory.CreateDbContext();

        _store.Clear();

        var entries = db.TConfigurations
            .TagWith("DatabaseConfigurationProvider: Full Load")
            .OrderBy(e => e.Key)
            .ToList();

        foreach (var e in entries)
        {
            _store[e.Key] = e.Value;
            if (e.LastUpdated > _lastChecked)
                _lastChecked = e.LastUpdated;
        }

        if (_lastChecked == DateTimeOffset.MinValue)
            _lastChecked = DateTimeOffset.UtcNow;

        _logger.LogInformation(
            "Full configuration load completed. Loaded {Count} entries.",
            entries.Count
        );
    }

    // ---------------------------------------------------------
    // Polling Logic
    // ---------------------------------------------------------

    public async Task CheckForUpdatesAsync(CancellationToken cancellation = default)
    {
        _logger.LogDebug("Checking for configuration updates since {LastChecked}.", _lastChecked);

        using var db = _dbFactory.CreateDbContext();

        var changedEntries = await db.TConfigurations
            .TagWith("DatabaseConfigurationProvider: Delta Load (LastUpdated > _lastChecked)")
            .Where(e => e.LastUpdated > _lastChecked)
            .OrderBy(e => e.Key)
            .ToListAsync(cancellation);

        if (changedEntries.Count == 0)
        {
            _logger.LogDebug("No configuration changes detected.");
            return;
        }

        foreach (var e in changedEntries)
        {
            _logger.LogInformation("Config changed: {Key}", e.Key);

            _store[e.Key] = e.Value;
            if (e.LastUpdated > _lastChecked)
                _lastChecked = e.LastUpdated;
        }

        _logger.LogInformation(
            "Delta load applied. Updated {Count} entries.",
            changedEntries.Count
        );
    }

    public async Task CheckAndReloadIfNeededAsync(CancellationToken cancellation = default)
    {
        var before = _lastChecked;

        await CheckForUpdatesAsync(cancellation).ConfigureAwait(false);

        if (_lastChecked > before)
        {
            _logger.LogInformation("Detected configuration changes. Reloading.");
            TriggerReload();
        }
    }

    // ---------------------------------------------------------
    // Change Token Trigger
    // ---------------------------------------------------------

    public void TriggerReload()
    {
        lock (_reloadLock)
        {
            _logger.LogDebug("Firing configuration reload token.");

            var oldCts = _reloadTokenSource;
            _reloadTokenSource = new CancellationTokenSource();

            try { oldCts.Cancel(); } catch { }
            oldCts.Dispose();
        }
    }

    // ---------------------------------------------------------
    // Cleanup
    // ---------------------------------------------------------

    public void Reload() => Load();

    public void Dispose()
    {
        if (_disposed) return;

        _logger.LogDebug("Disposing DatabaseConfigurationProvider.");

        _reloadTokenSource.Cancel();
        _reloadTokenSource.Dispose();
        _disposed = true;
    }
}
