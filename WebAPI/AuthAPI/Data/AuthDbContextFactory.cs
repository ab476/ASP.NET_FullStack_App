using Common.Constants;
using MySqlConnector;

namespace AuthAPI.Data;

public class AuthDbContextFactory(IMultiDatabaseConfig _dbOptions, [FromKeyedServices(ResourceNames.AuthDb)] MySqlDataSource authdb)
{

    public void Configure(DbContextOptionsBuilder options)
    {
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        var activeDb = DatabaseType.MySQL;// _dbOptions.ActiveDatabase;
        var connStr = authdb.ConnectionString; // _dbOptions.GetActiveConnectionString();

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

        if (activeDb is not DatabaseType.Oracle)
        {
            options.UseSnakeCaseNamingConvention();
        }
    }
}