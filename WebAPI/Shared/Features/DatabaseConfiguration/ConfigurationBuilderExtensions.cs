using Common.Features.DatabaseConfiguration.Endpoints;
using Common.Features.DatabaseConfiguration.Provider;
using EFCore.NamingConventions.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Features.DatabaseConfiguration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddDatabaseConfiguration(
        this IConfigurationBuilder builder,
        IServiceCollection services,
        Action<DatabaseConfigurationOptions>? configure = null)
    {
        var tempProvider = services.BuildServiceProvider();
        var logger = tempProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseConfigurationSetup");

        services.TryAddScoped<INameRewriter, SnakeCaseNameRewriter>();

        logger.LogInformation("Initializing database configuration module.");

        var options = new DatabaseConfigurationOptions()
        {
            PollInterval = TimeSpan.FromMinutes(5),
            EnablePooling = true,
            MapDbScriptRoute = true,
            DbScriptRoute = DatabaseConfigurationOptions.DefaultDbScriptRoute
        };
        configure?.Invoke(options);

        logger.LogDebug("DatabaseConfigurationOptions prepared: PollInterval={Interval}, EnablePooling={Pooling}.",
            options.PollInterval,
            options.EnablePooling);

        var configuration = tempProvider.GetRequiredService<IConfiguration>();

        logger.LogInformation("Configuring DbContextFactory for configuration store.");

        services.AddPooledDbContextFactory<ConfigurationDbContext>(dbOptions =>
        {
            dbOptions.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.ConfigureDbContext?.Invoke(dbOptions);
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

        if (options.MapDbScriptRoute)
        {
            var route = options.DbScriptRoute ?? DatabaseConfigurationOptions.DefaultDbScriptRoute;
            logger.LogInformation("Mapping database script route at {Route}.", route);
            var appBuilder = finalProvider.GetRequiredService<IEndpointRouteBuilder>();
            appBuilder.MapDatabaseConfigurationScript(route);
        }
        return builder;
    }
}
