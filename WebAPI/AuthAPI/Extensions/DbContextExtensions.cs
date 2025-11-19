namespace AuthAPI.Extensions;
//public class SqlServerAuthDbContext(DbContextOptions<AuthDbContext> options) : AuthDbContext(options) { }
//public class MySqlAuthDbContext(DbContextOptions<AuthDbContext> options) : AuthDbContext(options) { }
//public class SQLiteAuthDbContext(DbContextOptions<AuthDbContext> options) : AuthDbContext(options) { }


//public static class DbContextExtensions
//{
//    public static IServiceCollection AddConfiguredDbContext(this IServiceCollection services, IConfiguration configuration)
//    {
//        var provider = configuration["Database:Provider"];

//        // Register provider-specific DbContexts
//        services.AddDbContext<SqlServerAuthDbContext>(options =>
//            options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

//        services.AddDbContext<MySqlAuthDbContext>(options =>
//            options.UseMySql(configuration.GetConnectionString("MySql"),
//                             ServerVersion.AutoDetect(configuration.GetConnectionString("MySql"))));

//        // Determine the actual DbContext type once
//        Type baseDbContextType = provider switch
//        {
//            "SqlServer" => { 
//                return typeof(SqlServerAuthDbContext) 
//            },
//            "MySql" => typeof(MySqlAuthDbContext),
//            _ => throw new Exception("Invalid database provider in configuration")
//        };

//        // Register BaseDbContext as the resolved provider type
//        services.AddScoped(typeof(AuthDbContext), sp => sp.GetRequiredService(baseDbContextType));

//        return services;
//    }
//}