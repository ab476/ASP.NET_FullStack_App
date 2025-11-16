using AuthAPI.Data;
using AuthAPI.Data.TableConfigurations;
using AuthAPI.Services.SqlSchema;
using AuthAPI.Services.Time;
using EFCore.NamingConventions.Internal;

namespace AuthAPI.Extensions.ServiceCollectionExtensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<INameRewriter>(provider =>
        {
            var culture = CultureInfo.CurrentCulture;
            var dbConfig = provider.GetRequiredService<IMultiDatabaseConfig>();

            return dbConfig.ActiveDatabase switch
            {
                DatabaseType.Oracle => new UpperSnakeCaseNameRewriter(culture),
                _ => new SnakeCaseNameRewriter(culture),
            };
        });

        services.AddSingleton<IEntityConfigurationAggregator, EntityConfigurationAggregator>();
        services.AddSingleton<AuthDbContextFactory>();

        services.AddTimeProvider()
                .AddSchemaService();

        services.AddHttpClient();

        return services;
    }
}
