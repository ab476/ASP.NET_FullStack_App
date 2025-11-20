using AuthAPI.Infrastructure.Validation;
using AuthAPI.Services.UserAddresses.Contracts;
using AuthAPI.Services.UserAddresses.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Services.UserAddresses.Endpoints;

public static class UserAddressEndpoints
{
    public static IEndpointRouteBuilder MapUserAddressEndpoints(this IEndpointRouteBuilder app)
    {
        // -------------------------------------------
        // USER-SCOPED ENDPOINTS (userId required)
        // -------------------------------------------
        var userGroup = app.MapGroup("api/users/{userId:guid}/addresses")
                           .WithTags("User Addresses");

        userGroup.MapGet("/",
            async Task<Ok<IEnumerable<UserAddressResponse>>> (
                Guid userId,
                [FromServices] IUserAddressService service,
                CancellationToken ct) =>
            {
                var result = await service.GetByUserIdAsync(userId, ct);
                return TypedResults.Ok(result);
            }
        ).WithSummary("GET all addresses for a user");

        userGroup.MapPost("/", 
            async Task<Results<Ok<UserAddressResponse>, ValidationProblem>> (
                Guid userId,
                CreateUserAddressRequest request,
                [FromServices] IUserAddressService service,
                [FromServices] IValidator<CreateUserAddressRequest> validator,
                CancellationToken ct) =>
            {
                request.UserId = userId;
                var results = await validator.ValidateAsync(request, ct);
                if (!results.IsValid)
                {
                    return TypedResults.ValidationProblem(results.ToDictionary());
                }

                var created = await service.CreateAsync(request, ct);
                return TypedResults.Ok(created);
            }
        ).WithSummary("CREATE an address for a user");

        // -------------------------------------------
        // ADDRESS-SCOPED ENDPOINTS (no userId needed)
        // -------------------------------------------
        var addressGroup = app.MapGroup("api/addresses")
                              .WithTags("User Addresses");

        addressGroup.MapGet("/{id:guid}",
            async Task<Results<Ok<UserAddressResponse>, NotFound>> (
                Guid id,
                [FromServices] IUserAddressService service,
                CancellationToken ct) =>
            {
                var result = await service.GetByIdAsync(id, ct);
                return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
            }
        ).WithSummary("GET address by Id");

        addressGroup.MapPut("/{id:guid}", 
            async Task<Results<Ok, NotFound>>  (
                Guid id,
                UpdateUserAddressRequest request,
                [FromServices] IUserAddressService service,
                CancellationToken ct) =>
            {
                var updated = await service.UpdateAsync(id, request, ct);
                return updated  ? TypedResults.Ok() : TypedResults.NotFound();
            }
        )
        .WithValidation<UpdateUserAddressRequest>()
        .WithSummary("UPDATE address by Id");

        addressGroup.MapDelete("/{id:guid}", 
            async Task<Results<Ok, NotFound>> (
                Guid id,
                [FromServices] IUserAddressService service,
                CancellationToken ct) =>
            {
                var deleted = await service.DeleteAsync(id, ct);
                return deleted ? TypedResults.Ok() : TypedResults.NotFound();
            }
        ).WithSummary("DELETE address by Id");

        return app;
    }
}