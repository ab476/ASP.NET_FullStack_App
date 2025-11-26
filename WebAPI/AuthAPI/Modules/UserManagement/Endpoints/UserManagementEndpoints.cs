namespace AuthAPI.Modules.UserManagement.Endpoints;

public static class UserManagementEndpoints
{
    public static IEndpointRouteBuilder MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/users")
                       .RequireAuthorization("AdminOnly"); // optional policy

        // Get all users (paged recommended)
        group.MapGet("/", async (UserManager<IdentityUser> userManager) =>
        {
            var users = await userManager.Users
                .Select(u => new { u.Id, u.UserName, u.Email })
                .ToListAsync();

            return Results.Ok(users);
        });

        // Get single user
        group.MapGet("/{id}", async (string id, UserManager<IdentityUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);
            return user is null
                ? Results.NotFound()
                : Results.Ok(new { user.Id, user.UserName, user.Email });
        });

        // Create user
        group.MapPost("/", async (CreateUserRequest req, UserManager<IdentityUser> userManager) =>
        {
            var user = new IdentityUser
            {
                UserName = req.UserName,
                Email = req.Email
            };

            var result = await userManager.CreateAsync(user, req.Password);
            return result.Succeeded
                ? Results.Created($"/admin/users/{user.Id}", user.Id)
                : Results.BadRequest(result.Errors);
        });

        // Delete user
        group.MapDelete("/{id}", async (string id, UserManager<IdentityUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null) return Results.NotFound();

            var result = await userManager.DeleteAsync(user);
            return result.Succeeded ? Results.NoContent() : Results.BadRequest(result.Errors);
        });

        return app;
    }
}

// DTO
public record CreateUserRequest(string UserName, string Email, string Password);

