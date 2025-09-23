namespace Shared.Middleware;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Net;
using System.Net.Mime;
using System.Text.Json;


/// <summary>
/// Middleware for centralized exception handling in ASP.NET Core applications.
/// </summary>
/// <remarks>
/// This middleware intercepts unhandled exceptions in the request pipeline,
/// logs them, and returns a standardized JSON response with an appropriate
/// HTTP status code. It helps ensure consistent error handling across APIs.
/// </remarks>
internal class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    /// <summary>
    /// Executes the middleware logic. Invokes the next delegate in the pipeline and
    /// catches any exceptions that occur during request processing.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case UnauthorizedAccessException:
                    await HandleExceptionAsync(context, ex, HttpStatusCode.Forbidden);
                    break;
                case ArgumentException:
                    await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
                    break;
                case KeyNotFoundException:
                    await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
                    break;
                case DbUpdateConcurrencyException:
                    await HandleExceptionAsync(context, ex, HttpStatusCode.Conflict, "Database concurrency conflict");
                    break;
                case DbUpdateException:
                    await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Database update failed");
                    break;
                default:
                    if (ex.GetBaseException() is DbException)
                        await HandleExceptionAsync(context, ex, HttpStatusCode.ServiceUnavailable, "Database connection error");
                    else
                        await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "An unexpected error occurred");
                    break;
            }
        }
    }

    /// <summary>
    /// Handles an exception by logging it and writing a standardized JSON error response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="ex">The exception that was thrown.</param>
    /// <param name="statusCode">The HTTP status code to return.</param>
    /// <param name="customMessage">An optional custom error message for the response.</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode, string? customMessage = null)
    {
        logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = (int)statusCode;

        var response = new ApiResponse<object>(
            Success: false,
            Data: null,
            Message: customMessage ?? "An unexpected error occurred"
        );

        var json = JsonSerializer.Serialize(response, ApiResponseJsonContext.Default.ApiResponseObject);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension methods for registering the <see cref="ExceptionHandlingMiddleware"/>.
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Registers the <see cref="ExceptionHandlingMiddleware"/> in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddExceptionHandlingMiddleware(this IServiceCollection services)
    {
        return services.AddTransient<ExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Adds the <see cref="ExceptionHandlingMiddleware"/> to the application pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder with middleware registered.</returns>
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

