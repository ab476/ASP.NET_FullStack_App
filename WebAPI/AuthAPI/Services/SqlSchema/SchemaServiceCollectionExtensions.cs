using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthAPI.Services.SqlSchema;

public static class SchemaServiceCollectionExtensions
{
    public static IServiceCollection AddSchemaService(this IServiceCollection services)
    {

        services.TryAddScoped<ISqlSchemaParser, SqlSchemaParserService>();

        return services;
    }
}
