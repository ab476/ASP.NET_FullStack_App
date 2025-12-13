//using Common.Constants;
//using Common.Features.DatabaseConfiguration.Provider;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using MySqlConnector;
//using System.Globalization;

//namespace Common.Features.DatabaseConfiguration;

//public static class ConfigurationBuilderExtensions
//{
//    public static IConfigurationBuilder AddDatabaseConfiguration(
//        this IConfigurationBuilder builder,
//        IServiceCollection services,
//        Action<DatabaseConfigurationOptions> configureOptions)
//    {
//        // Register and bind options using standard .NET patterns
//        services.Configure(configureOptions);

//        // Register refresh interval provider (lazy resolution from IOptions instead of manual creation)
//        services.AddSingleton<IConfigRefreshInterval>(sp =>
//        {
//            var opts = sp.GetRequiredService<IOptions<DatabaseConfigurationOptions>>().Value;
//            return new StaticConfigRefreshIntervalProvider(opts.PollInterval);
//        });

//        // Register provider only once
//        services.AddSingleton<DatabaseConfigurationProvider>();

//        // Hosted service only if polling is enabled
//        services.AddHostedService(sp =>
//        {
//            var opts = sp.GetRequiredService<IOptions<DatabaseConfigurationOptions>>().Value;
//            var logger = sp.GetRequiredService<ILoggerFactory>()
//                           .CreateLogger("DatabaseConfigurationSetup");

//            if (!opts.EnablePooling)
//            {
//                logger.LogWarning("Database configuration polling is disabled.");
//                return new DisabledHostedService();
//            }

//            return new DatabaseConfigurationHostedService(
//                sp.GetRequiredService<DatabaseConfigurationProvider>(),
//                sp.GetRequiredService<IConfigRefreshInterval>(),
//                logger);
//        });

//        // Register our DbContextFactory using correct DI patterns
//        services.AddPooledDbContextFactory<ConfigurationDbContext>((sp, dbOptions) =>
//        {
//            var opts = sp.GetRequiredService<IOptions<DatabaseConfigurationOptions>>().Value;
//            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbConfig");

//            var configDb = sp.GetRequiredKeyedService<MySqlDataSource>(ResourceNames.ConfigurationDb);
//            var connectionString = configDb.ConnectionString;

//            dbOptions
//                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
//                .UseSnakeCaseNamingConvention(CultureInfo.InvariantCulture)
//                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

//            // Allow user-defined customizations
//            opts.ConfigureDbContext?.Invoke(dbOptions);

//            logger.LogInformation("ConfigurationDbContext configured (pooling={Pooling}).",
//                opts.EnablePooling);
//        });

//        // Add configuration source WITHOUT resolving services early
//        builder.Add(new DatabaseConfigurationSource(sp =>
//        {
//            var logger = sp.GetRequiredService<ILoggerFactory>()
//                           .CreateLogger("DatabaseConfigurationSetup");

//            logger.LogInformation("DatabaseConfigurationProvider added to configuration pipeline.");

//            return sp.GetRequiredService<DatabaseConfigurationProvider>();
//        }));

//        return builder;
//    }

//    /// <summary>
//    /// Used when polling is disabled to satisfy AddHostedService requirement.
//    /// </summary>
//    private sealed class DisabledHostedService : IHostedService
//    {
//        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
//        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
//    }
//}
