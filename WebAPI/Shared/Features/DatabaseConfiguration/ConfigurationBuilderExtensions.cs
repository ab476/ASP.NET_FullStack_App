using Common.Features.DatabaseConfiguration.Provider;

namespace Common.Features.DatabaseConfiguration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddDatabaseConfiguration(
        this IConfigurationBuilder builder,
        IServiceCollection services,
        Action<DatabaseConfigurationOptions> configure)
    {
        var tempProvider = services.BuildServiceProvider();
        var logger = tempProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseConfigurationSetup");

        logger.LogInformation("Initializing database configuration module.");

        var options = new DatabaseConfigurationOptions()
        {
            PollInterval = TimeSpan.FromMinutes(5),
            EnablePooling = true,
            DbScriptRoute = DatabaseConfigurationOptions.DefaultDbScriptRoute
        };
        configure.Invoke(options);

        logger.LogDebug("DatabaseConfigurationOptions prepared: PollInterval={Interval}, EnablePooling={Pooling}.",
            options.PollInterval,
            options.EnablePooling);

        var configuration = tempProvider.GetRequiredService<IConfiguration>();

        logger.LogInformation("Configuring DbContextFactory for configuration store.");

        services.AddPooledDbContextFactory<ConfigurationDbContext>(dbOptions =>
        {
            dbOptions.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.ConfigureDbContext?.Invoke(dbOptions);

            if(dbOptions.IsConfigured)
            {
                logger.LogInformation("DbContextOptions have been configured.");
            }
            else
            {
                logger.LogError("DbContextOptions have not been configured. Please ensure ConfigureDbContext is set in DatabaseConfigurationOptions.");
            }
        });
        

        logger.LogInformation("Registered IConfigRefreshInterval with interval {Interval}.", options.PollInterval);
        services.AddSingleton<IConfigRefreshInterval>(new StaticConfigRefreshIntervalProvider(options.PollInterval));

        logger.LogInformation("Registering DatabaseConfigurationProvider.");
        services.AddSingleton<DatabaseConfigurationProvider>();

        if (options.EnablePooling)
        {
            logger.LogInformation("Registering DatabaseConfigurationHostedService for background polling.");
            services.AddHostedService<DatabaseConfigurationHostedService>();
        }
        else
        {
            logger.LogWarning("Polling service is disabled. Configuration updates will not be auto-refreshed.");
        }

        var finalProvider = services.BuildServiceProvider();
        var configProvider = finalProvider.GetRequiredService<DatabaseConfigurationProvider>();

        logger.LogInformation("Adding DatabaseConfigurationProvider to configuration pipeline.");
        builder.Add(new DatabaseConfigurationSource(configProvider));

        logger.LogInformation("Database configuration setup completed successfully.");
        
        return builder;
    }
}
