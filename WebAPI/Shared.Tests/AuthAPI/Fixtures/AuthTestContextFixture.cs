using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Tests.AuthAPI.Fixtures;

public class AuthTestContextFixture : IAuthTestContext
{
    private readonly IAuthTestContext _impl;

    public AuthTestContextFixture()
    {
        var db = Environment.GetEnvironmentVariable("TEST_DB") ?? "mysql";

        _impl = db switch
        {
            "mysql" => new MySqlAuthTestContext(),
            "postgres" => new PostgreSqlAuthTestContext(),
            "mssql" => new SqlServerAuthTestContext(),
            _ => throw new InvalidOperationException($"Unknown TEST_DB '{db}'"),
        };
    }

    public HttpClient Client => _impl.Client;
    public AuthWebApplicationFactory Factory => _impl.Factory;

    public Task InitializeAsync() => _impl.InitializeAsync();
    public Task DisposeAsync() => _impl.DisposeAsync();
}

