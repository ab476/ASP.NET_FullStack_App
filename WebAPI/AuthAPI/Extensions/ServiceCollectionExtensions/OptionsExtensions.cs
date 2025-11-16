namespace AuthAPI.Extensions.ServiceCollectionExtensions;

public static class OptionsExtensions
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptionsFromConfiguration<IMultiDatabaseConfig, MultiDatabaseConfig>("MultiDatabaseConfig");
        services.AddOptionsFromConfiguration<IEmailSettings, EmailSettings>("EmailSettings");

        return services;
    }
}
