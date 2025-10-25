namespace AuthAPI.BackgroundService;

using AuthAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class InitializeDatabaseService(
    IServiceScopeFactory scopeFactory,
    IHostEnvironment env,
    ILogger<InitializeDatabaseService> logger
) : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IHostEnvironment _env = env;
    private readonly ILogger<InitializeDatabaseService> _logger = logger;

    private const string AdminEmail = "admin@example.com";
    private const string MemberEmail = "member@example.com";
    private const string AdminRole = "Admin";
    private const string DefaultPassword = "password";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_env.IsDevelopment())
        {
            _logger.LogInformation("Skipping database initialization in {Environment}", _env.EnvironmentName);
            return;
        }

        _logger.LogInformation("Initializing database and Identity data...");

        await using var scope = _scopeFactory.CreateAsyncScope();
        var provider = scope.ServiceProvider;

        var dbContext = provider.GetRequiredService<AuthDbContext>();
        var userManager = provider.GetRequiredService<UserManager<TUser>>();
        var roleManager = provider.GetRequiredService<RoleManager<TRole>>();

        await EnsureDatabaseCreatedAsync(dbContext, cancellationToken);
        await EnsureRoleExistsAsync(roleManager, AdminRole);

        await EnsureUserExistsAsync(userManager, AdminEmail, AdminRole);
        await EnsureUserExistsAsync(userManager, MemberEmail);

        _logger.LogInformation("Initialization completed successfully.");
    }

    private async Task EnsureDatabaseCreatedAsync(AuthDbContext dbContext, CancellationToken ct)
    {
        _logger.LogInformation("Ensuring database is created...");
        await dbContext.Database.EnsureDeletedAsync(ct);
        await dbContext.Database.EnsureCreatedAsync(ct);
    }

    private async Task EnsureRoleExistsAsync(RoleManager<TRole> roleManager, string role)
    {
        if (await roleManager.RoleExistsAsync(role))
        {
            _logger.LogDebug("Role already exists: {Role}", role);
            return;
        }

        await roleManager.CreateAsync(new TRole { Name = role });
        _logger.LogInformation("Created role: {Role}", role);
    }

    private async Task EnsureUserExistsAsync(UserManager<TUser> userManager, string email, string? role = null)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            _logger.LogDebug("User already exists: {Email}", email);
            return;
        }

        var newUser = new TUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(newUser, DefaultPassword);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed creating user {Email}: {Errors}", email,
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        _logger.LogInformation("Created user: {Email}", email);

        if (role is not null)
        {
            await userManager.AddToRoleAsync(newUser, role);
            _logger.LogInformation("Assigned role {Role} to {Email}", role, email);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("InitializeDatabaseService stopped.");
        return Task.CompletedTask;
    }
}
