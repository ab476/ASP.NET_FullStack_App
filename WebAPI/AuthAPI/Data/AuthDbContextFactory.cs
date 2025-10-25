using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Data;

public class AuthDbContextFactory(MultiDatabaseConfig _dbOptions)
{

    public void Configure(DbContextOptionsBuilder options)
    {
        var activeDb = _dbOptions.ActiveDatabase;
        var connStr = _dbOptions.GetActiveConnectionString();

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
            {
                options.UseOracle(connStr)
                        .UseUpperSnakeCaseNamingConvention();
                break;
            }
            default:
                throw new InvalidOperationException($"Unsupported database: {activeDb}");
        }

        if(activeDb is not DatabaseType.Oracle)
        {
            options.UseSnakeCaseNamingConvention();
        }
    }
}
