using Common.Features.DatabaseConfiguration.Provider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Net.Mime;

namespace Common.Features.DatabaseConfiguration;
public static class DatabaseConfigurationEndpointExtensions
{
    public static void MapDatabaseConfigurationEndpoints(this IEndpointRouteBuilder endpoints, DatabaseConfigurationOptions? options = null)
    {
            var route = options?.DbScriptRoute ?? DatabaseConfigurationOptions.DefaultDbScriptRoute;

            endpoints
                .MapGet(route, async (IDbContextFactory<ConfigurationDbContext> _dbFactory) =>
                {
                    using var db = await _dbFactory.CreateDbContextAsync();
                    var script = db.Database.GenerateCreateScript();

                    return Results.Text(script, MediaTypeNames.Text.Plain);
                })
                .WithName("GetConfigurationDbScript")
                .WithSummary("Returns CREATE TABLE DDL for config system")
                .WithTags("Database Configuration")
                .WithOpenApi();
    }
}

