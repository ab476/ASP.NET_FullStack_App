using AuthAPI.Data;
using Testcontainers.MsSql;

namespace Shared.Tests.AuthAPITests.Fixtures;

public class SqlServerAuthTestContext : IAuthTestContext
{
    private readonly MsSqlContainer _container;

    public HttpClient Client { get; private set; } = default!;
    public AuthWebApplicationFactory Factory { get; private set; } = default!;

    public FakeTimeProvider TimeProvider => throw new NotImplementedException();

    public SqlServerAuthTestContext()
    {
        _container = new MsSqlBuilder()
            .WithPassword("Password123!")
            .WithReuse(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        Factory = new AuthWebApplicationFactory(_container.GetConnectionString(), "");
        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();

        Client.Dispose();
        Factory.Dispose();
    }

    public AuthDbContext CreateDbContext()
    {
        throw new NotImplementedException();
    }
}

