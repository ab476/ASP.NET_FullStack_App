namespace AppHost.Resources.Database;

public class DatabaseOptions
{
    public const string Database = "Database";
    public DatabaseProvider Provider { get; set; } = DatabaseProvider.SqlServer;
}
