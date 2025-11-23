using AuthAPI.Data;
using Common.Constants;
using MySqlConnector;
using System.Globalization;
using Testcontainers.MySql;

namespace Shared.Tests.AuthAPITests.Fixtures;

public class MySqlAuthTestContext : IAuthTestContext
{
    private readonly MySqlContainer _container;
    private string _baseConnectionString = default!;
    private DbContextOptions<AuthDbContext> _authDbOptions = default!;
    private string _authDbString = default!;
    private ServerVersion _authDbVersion = default!;

    private readonly string[] _databases = [
        ResourceNames.ConfigurationDb,
        ResourceNames.AuthDb
    ];

    public HttpClient Client { get; private set; } = default!;
    public AuthWebApplicationFactory Factory { get; private set; } = default!;

    public FakeTimeProvider TimeProvider { get; private set; } = new();

    public MySqlAuthTestContext()
    {
        _container = new MySqlBuilder()
            .WithUsername("root")
            .WithPassword("Password123!")
            .WithReuse(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _baseConnectionString = _container.GetConnectionString();

        await EnsureDatabasesExistAsync();

        _authDbString = GetConnectionStringForDb(ResourceNames.AuthDb);
        _authDbVersion = ServerVersion.AutoDetect(_authDbString);

        _authDbOptions = new DbContextOptionsBuilder<AuthDbContext>()
            .UseMySql(_authDbString, _authDbVersion)
            .UseSnakeCaseNamingConvention(CultureInfo.CurrentCulture)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

        Factory = new AuthWebApplicationFactory(
            _authDbString,
            GetConnectionStringForDb(ResourceNames.ConfigurationDb)
        );

        Client = Factory.CreateClient();
    }
    private async Task EnsureDatabasesExistAsync()
    {
        using var conn = new MySqlConnection(_baseConnectionString);
        await conn.OpenAsync();
        foreach (var db in _databases)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{db}`;";
            await cmd.ExecuteNonQueryAsync();
        }
    }
    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();

        Client.Dispose();
        Factory.Dispose();
    }
    /// <summary>
    /// Creates a fresh AppDbContext for each test method.
    /// </summary>
    public AuthDbContext CreateDbContext()
    {
        return new AuthDbContext(_authDbOptions);
    }
    private string GetConnectionStringForDb(string database)
    {
        var builder = new MySqlConnectionStringBuilder(_baseConnectionString)
        {
            Database = database
        };

        return builder.ToString();
    }

}