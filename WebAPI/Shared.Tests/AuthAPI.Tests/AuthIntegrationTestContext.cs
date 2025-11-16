using Testcontainers.MySql;

namespace Shared.Tests.AuthAPI.Tests;
public class AuthIntegrationTestContext : IAsyncLifetime
{
    public readonly MySqlContainer AuthDbContainer;
    public HttpClient Client { get; private set; } = default!;
    public AuthWebApplicationFactory Factory { get; private set; } = default!;

    public AuthIntegrationTestContext()
    {
        AuthDbContainer = new MySqlBuilder()
            .WithDatabase("AuthTestDb")
            .WithUsername("root")
            .WithPassword("Password123!")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await AuthDbContainer.StartAsync();

        Factory = new AuthWebApplicationFactory(AuthDbContainer.GetConnectionString());

        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await AuthDbContainer.StopAsync();
        await AuthDbContainer.DisposeAsync();

        Client.Dispose();
        Factory.Dispose();
    }
}

