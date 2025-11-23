using Common.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Tests.AuthAPITests;

public class AuthWebApplicationFactory(string connectionString, string configDb) : WebApplicationFactory<Program>
{
    private readonly string _connectionString = connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Mark this execution as TEST environment
        builder.UseSetting("TEST_ENV", "true");
        builder.UseSetting($"ConnectionStrings:{ResourceNames.AuthDb}", _connectionString);
        builder.UseSetting($"ConnectionStrings:{ResourceNames.ConfigurationDb}", configDb);

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { $"ConnectionStrings:{ResourceNames.AuthDb}", _connectionString },
                { $"ConnectionStrings:{ResourceNames.ConfigurationDb}", configDb },
            });
        });

        builder.ConfigureTestServices(async services =>
        {
            services.PostConfigure<IConfiguration>(configuration =>
            {
                configuration[$"ConnectionStrings:{ResourceNames.AuthDb}"] = _connectionString;
                configuration[$"ConnectionStrings:{ResourceNames.ConfigurationDb}"] = configDb;
            });
            
            //services.AddDbContext<AuthDbContext>(options =>
                //options.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString)));

            // Apply migrations
            using var scope = services.BuildServiceProvider().CreateScope();

            //var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            //await db.Database.EnsureDeletedAsync();
            //await db.Database.EnsureCreatedAsync();
        });
    }
}

