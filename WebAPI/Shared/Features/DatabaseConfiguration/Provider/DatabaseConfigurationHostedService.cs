using Microsoft.Extensions.Hosting;

namespace Common.Features.DatabaseConfiguration.Provider;

public sealed class DatabaseConfigurationHostedService(
    DatabaseConfigurationProvider provider,
    IConfigRefreshInterval intervalProvider,
    ILogger<DatabaseConfigurationHostedService> logger) : BackgroundService
{
    private readonly TimeSpan _maxJitter = TimeSpan.FromSeconds(5); // optional jitter cap

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("DatabaseConfigurationHostedService starting.");

        // Initial load
        try
        {
            provider.Load();
            logger.LogInformation("Initial configuration load completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Initial configuration load failed.");
        }

        // Cache interval locally unless you expect it to change dynamically.
        var interval = intervalProvider.Value;
        if (interval <= TimeSpan.Zero)
        {
            logger.LogWarning("Configured interval is zero or negative — polling disabled.");
            return;
        }

        var rand = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await provider.CheckAndReloadIfNeededAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while checking for DB configuration updates.");
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

        logger.LogInformation("DatabaseConfigurationHostedService stopping.");
    }
}
