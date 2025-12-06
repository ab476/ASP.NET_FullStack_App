using Microsoft.Extensions.Configuration;
namespace AppHost.Resources.Database;

public static class DatabaseContainerExtensions
{
    public static DatabaseResourceSetWithOptions ConfigureDatabaseContainers(this IDistributedApplicationBuilder builder)
    {
        var dbOptions = builder.Configuration
            .GetSection(DatabaseOptions.Database)
            .Get<DatabaseOptions>()
            ?? throw new InvalidOperationException("Database configuration not found.");

        DatabaseResourceSet databaseResourceSet = dbOptions.Provider switch
        {
            DatabaseProvider.SqlServer => CreateSqlServer(builder),
            DatabaseProvider.Postgres => CreatePostgres(builder),
            DatabaseProvider.MySql => CreateMySql(builder),
            DatabaseProvider.Oracle => CreateOracle(builder),

            _ => throw new InvalidOperationException(
                    $"Unsupported database provider: {dbOptions.Provider}")
        };

        return new DatabaseResourceSetWithOptions(
            dbOptions,
            databaseResourceSet.Instance,
            databaseResourceSet.ConfigurationDb,
            databaseResourceSet.AuthDb
        );
    }
        

    private static DatabaseResourceSet CreateSqlServer(IDistributedApplicationBuilder builder)
    {
        var sql = builder.AddSqlServer(ResourceNames.SqlServer)
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDbGate()
            .WithAdminer();

        var configurationDb = sql.AddDatabase(ResourceNames.ConfigurationDb);
        var authDb = sql.AddDatabase(ResourceNames.AuthDb);

        return new DatabaseResourceSet(sql, configurationDb, authDb);
    }

    private static DatabaseResourceSet CreatePostgres(IDistributedApplicationBuilder builder)
    {
        var pg = builder.AddPostgres(ResourceNames.Postgres)
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDbGate()
            .WithAdminer();

        var configurationDb = pg.AddDatabase(ResourceNames.ConfigurationDb);
        var authDb = pg.AddDatabase(ResourceNames.AuthDb);

        return new DatabaseResourceSet(pg, configurationDb, authDb);
    }

    private static DatabaseResourceSet CreateMySql(IDistributedApplicationBuilder builder)
    {
        var mysql = builder.AddMySql(ResourceNames.MySql)
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDbGate()
            .WithAdminer();

        var configurationDb = mysql.AddDatabase(ResourceNames.ConfigurationDb);
        var authDb = mysql.AddDatabase(ResourceNames.AuthDb);

        return new DatabaseResourceSet(mysql, configurationDb, authDb);
    }

    private static DatabaseResourceSet CreateOracle(IDistributedApplicationBuilder builder)
    {
        var oracle = builder.AddOracle(ResourceNames.Oracle)
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent);

        var configurationDb = oracle.AddDatabase(ResourceNames.ConfigurationDb);
        var authDb = oracle.AddDatabase(ResourceNames.AuthDb);

        return new DatabaseResourceSet(oracle, configurationDb, authDb);
    }
}
