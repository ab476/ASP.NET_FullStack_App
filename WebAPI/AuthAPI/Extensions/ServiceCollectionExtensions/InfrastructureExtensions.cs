using AuthAPI.Services.SqlSchema;

namespace AuthAPI.Extensions.ServiceCollectionExtensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<AuthDbContextFactory>();

        services.AddTimeProvider()
                .AddSchemaService();

        services.AddHttpClient();

        return services;
    }
}