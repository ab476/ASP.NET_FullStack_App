using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace AuthAPI.Endpoints;

public static class KeyGenEndpoints
{
    public static IEndpointRouteBuilder MapKeyGenEndpoints(this IEndpointRouteBuilder app)
    {
        var env = app.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if(!env.IsDevelopment())
        {
            return app;
        }

        var group = app.MapGroup("/api/keygen")
            .WithTags("KeyGen");

        group.MapGet("/", ([AsParameters] KeyGenRequest request, [FromServices] ITimeProvider timeProvider) =>
        {
            if (request.Size < 16 || request.Size > 256)
                return Results.BadRequest("Key size must be between 16 and 256 bytes.");

            var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(request.Size));

            return Results.Ok(new
            {
                key,
                bytes = request.Size,
                bits = request.Size * 8,
                generatedAt = timeProvider.UtcNow
            });
        })
        .WithSummary("Generates a secure Base64 key for JWT or encryption use.")
        .WithDescription("Specify the key size (in bytes). Defaults to 64 bytes.");

        return app;
    }

    private record KeyGenRequest(int Size = 64);
}