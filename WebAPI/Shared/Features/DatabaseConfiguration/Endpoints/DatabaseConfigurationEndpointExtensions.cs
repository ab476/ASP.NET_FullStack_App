using Common.Features.DatabaseConfiguration.Provider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Mime;

namespace Common.Features.DatabaseConfiguration.Endpoints;

public static class DatabaseConfigurationEndpointExtensions
{
    public static void MapDatabaseConfigurationEndpoints(
        this IEndpointRouteBuilder endpoints,
        DatabaseConfigurationOptions? options = null)
    {
        var route = options?.DbScriptRoute ?? DatabaseConfigurationOptions.DefaultDbScriptRoute;

        var group = endpoints.MapGroup("/_internal/config-db")
            .WithTags("Database Configuration (Internal)");

        group.MapGet(route, Handler)
            .WithName("GetConfigurationDbScript")
            .WithSummary("Returns the CREATE TABLE DDL for the configuration system (development only).")
            .RequireAuthorization("Admin")
            .ExcludeFromDescription();
    }

    private static async Task<Results<ContentHttpResult, StatusCodeHttpResult>> Handler(
        IDbContextFactory<ConfigurationDbContext> dbFactory,
        ILoggerFactory loggerFactory,
        IWebHostEnvironment env)
    {
        var logger = loggerFactory.CreateLogger("DatabaseConfigurationDDL");

        // Restrict to Development environment
        if (!env.IsDevelopment())
        {
            logger.LogWarning("Attempted access to DDL endpoint outside development environment.");
            return TypedResults.StatusCode(StatusCodes.Status403Forbidden);
        }

        logger.LogInformation("Generating CREATE TABLE DDL for configuration database.");

        using var db = await dbFactory.CreateDbContextAsync();
        var script = db.Database.GenerateCreateScript();

        return TypedResults.Text(script, MediaTypeNames.Text.Plain);
    }
}
