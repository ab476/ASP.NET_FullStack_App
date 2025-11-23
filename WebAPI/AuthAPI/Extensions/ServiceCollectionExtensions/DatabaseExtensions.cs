using AuthAPI.BackgroundService;

namespace AuthAPI.Extensions.ServiceCollectionExtensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        //if (!configuration.IsTestEnvironment())
        //{
            services.AddDbContextPool<AuthDbContext>((sp, options) =>
            {
                var factory = sp.GetRequiredService<AuthDbContextFactory>();
                factory.Configure(options);
            });

            services.AddHostedService<InitializeDatabaseService>();
        //}


        return services;
    }
}