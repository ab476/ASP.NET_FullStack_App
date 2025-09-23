using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace AuthAPI.Data;

public class AuthDbContextFactory(IOptions<DatabaseOptions> options)
{

    private readonly DatabaseOptions _dbOptions = options.Value;

    public void Configure(DbContextOptionsBuilder options)
    {
        var activeDb = _dbOptions.ActiveDatabase;
        var connStr = _dbOptions.ConnectionStrings[activeDb.ToString()];

        switch (activeDb)
        {
            case DatabaseType.SQLServer:
                options.UseSqlServer(connStr);
                break;

            case DatabaseType.MySQL:
                options.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
                break;

            case DatabaseType.Postgres:
                options.UseNpgsql(connStr);
                break;

            case DatabaseType.SQLite:
                options.UseSqlite(connStr);
                break;

            case DatabaseType.Oracle:
                options.UseOracle(connStr);
                break;
            default:
                throw new InvalidOperationException($"Unsupported database: {activeDb}");
        }
    }
}
