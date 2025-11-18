using AuthAPI.Data;
using AuthAPI.Services.SqlSchema;
using AuthAPI.Services.Time;
using Common.Features.NameHelper;
using EFCore.NamingConventions.Internal;

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
