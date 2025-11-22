using Shared.Tests.AuthAPI;
using Testcontainers.MySql;

namespace Shared.Tests.AuthAPI.Fixtures;

public class MySqlAuthTestContext : IAuthTestContext
{
    private readonly MySqlContainer _container;

    public HttpClient Client { get; private set; } = default!;
    public AuthWebApplicationFactory Factory { get; private set; } = default!;

    public MySqlAuthTestContext()
    {
        _container = new MySqlBuilder()
            .WithDatabase("AuthTestDb")
            .WithUsername("root")
            .WithPassword("Password123!")
            .WithReuse(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        Factory = new AuthWebApplicationFactory(_container.GetConnectionString());
        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();

        Client.Dispose();
        Factory.Dispose();
    }
}