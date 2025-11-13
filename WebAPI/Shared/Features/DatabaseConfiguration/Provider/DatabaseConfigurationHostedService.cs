using Microsoft.Extensions.Hosting;

namespace Common.Features.DatabaseConfiguration.Provider;

public sealed class DatabaseConfigurationHostedService : BackgroundService
{
    private readonly DatabaseConfigurationProvider _provider;
    private readonly IConfigRefreshInterval _intervalProvider;
    private readonly ILogger<DatabaseConfigurationHostedService> _logger;
    private readonly TimeSpan _maxJitter; // optional jitter cap

    public DatabaseConfigurationHostedService(
        DatabaseConfigurationProvider provider,
        IConfigRefreshInterval intervalProvider,
        ILogger<DatabaseConfigurationHostedService> logger,
        TimeSpan? maxJitter = null)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _intervalProvider = intervalProvider ?? throw new ArgumentNullException(nameof(intervalProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _maxJitter = maxJitter ?? TimeSpan.FromSeconds(5); // default small jitter
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DatabaseConfigurationHostedService starting.");

        // Initial load
        try
        {
            _provider.Load();
            _logger.LogInformation("Initial configuration load completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Initial configuration load failed.");
        }

        // Cache interval locally unless you expect it to change dynamically.
        var interval = _intervalProvider.Value;
        if (interval <= TimeSpan.Zero)
        {
            _logger.LogWarning("Configured interval is zero or negative — polling disabled.");
            return;
        }

        var rand = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _provider.CheckAndReloadIfNeededAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking for DB configuration updates.");
            }

            // add jitter to reduce thundering herd in multi-instance deployments
            var jitterMs = rand.Next((int)_maxJitter.TotalMilliseconds + 1);
            var delay = interval + TimeSpan.FromMilliseconds(jitterMs);

            try
            {
                await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("DatabaseConfigurationHostedService stopping.");
    }
}
