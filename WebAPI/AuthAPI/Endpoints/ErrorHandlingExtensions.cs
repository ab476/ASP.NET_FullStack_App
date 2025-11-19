using Microsoft.AspNetCore.Diagnostics;

namespace AuthAPI.Endpoints;

public static class ErrorHandlingExtensions
{
    public static IEndpointRouteBuilder MapGlobalErrorEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.Map("/error", (HttpContext http) =>
        {
            var feature = http.Features.Get<IExceptionHandlerFeature>();
            var exception = feature?.Error;

            var logger = http.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("GlobalExceptionHandler");

            logger.LogError(exception, "An unhandled exception occurred.");

            return Results.Problem(
                title: "An unexpected error occurred.",
                statusCode: StatusCodes.Status500InternalServerError);
        })
        .AllowAnonymous(); // error endpoint should not require auth

        return endpoints;
    }
}