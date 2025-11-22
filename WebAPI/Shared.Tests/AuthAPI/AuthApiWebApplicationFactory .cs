using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AuthAPI.Data;

namespace Shared.Tests.AuthAPI;

public class AuthWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    private readonly string _connectionString = connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Mark this execution as TEST environment
        builder.UseSetting("TEST_ENV", "true");

        builder.ConfigureServices(async services =>
        {
            
            services.AddDbContext<AuthDbContext>(options =>
                options.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString)));

            // Apply migrations
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            await db.Database.EnsureCreatedAsync();
        });
    }
}

