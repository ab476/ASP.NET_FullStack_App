using Common.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Shared.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly ExceptionHandlingMiddleware _middleware;
    private readonly DefaultHttpContext _context;

    public ExceptionHandlingMiddlewareTests()
    {
        var logger = LoggerFactory.Create(builder => builder.AddConsole())
                                  .CreateLogger<ExceptionHandlingMiddleware>();
        _middleware = new ExceptionHandlingMiddleware(logger);
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
    }

    [Theory]
    [InlineData(typeof(UnauthorizedAccessException), HttpStatusCode.Forbidden, "An unexpected error occurred")]
    [InlineData(typeof(ArgumentException), HttpStatusCode.BadRequest, "An unexpected error occurred")]
    [InlineData(typeof(KeyNotFoundException), HttpStatusCode.NotFound, "An unexpected error occurred")]
    public async Task InvokeAsync_ShouldReturnCorrectStatus_ForKnownExceptions(Type exceptionType, HttpStatusCode expectedStatus, string expectedMessage)
    {
        // Arrange
        Task next(HttpContext ctx) => throw (Exception)Activator.CreateInstance(exceptionType)!;

        // Act
        await _middleware.InvokeAsync(_context, next);

        // Assert
        _context.Response.StatusCode.Should().Be((int)expectedStatus);

        _context.Response.Body.Position = 0;
        using var reader = new StreamReader(_context.Response.Body);
        var json = await reader.ReadToEndAsync();
        var response = JsonSerializer.Deserialize(json, ApiResponseJsonContext.Default.ApiResponseObject);

        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.Should().Be(expectedMessage);
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnInternalServerError_ForUnknownException()
    {
        // Arrange
        static Task next(HttpContext ctx) => throw new InvalidOperationException("Test error");

        // Act
        await _middleware.InvokeAsync(_context, next);

        // Assert
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

        _context.Response.Body.Position = 0;
        using var reader = new StreamReader(_context.Response.Body);
        var json = await reader.ReadToEndAsync();
        var response = JsonSerializer.Deserialize(json, ApiResponseJsonContext.Default.ApiResponseObject);

        response!.Message.Should().Be("An unexpected error occurred");
    }
}
