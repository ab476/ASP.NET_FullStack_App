using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Testcontainers.MySql;

namespace Shared.Tests.AuthAPI.Fixtures;

public class SqlServerAuthTestContext : IAuthTestContext
{
    private readonly MsSqlContainer _container;

    public HttpClient Client { get; private set; } = default!;
    public AuthWebApplicationFactory Factory { get; private set; } = default!;

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

