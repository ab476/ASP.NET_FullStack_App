namespace Common.Configurations;

public interface IMultiDatabaseConfig
{
    DatabaseType ActiveDatabase { get; set; }
    Dictionary<string, string> ConnectionStrings { get; set; }
}

public class MultiDatabaseConfig : IMultiDatabaseConfig
{
    public DatabaseType ActiveDatabase { get; set; }
    public Dictionary<string, string> ConnectionStrings { get; set; } = [];
}

public static partial class DatabaseOptionsExtensions
{
    public static string GetActiveConnectionString(this IMultiDatabaseConfig databaseOptions)
    {
        var activeDbKey = databaseOptions.ActiveDatabase.ToString();
        databaseOptions.ConnectionStrings.TryGetValue(activeDbKey, out string? connectionString);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Connection string for '{activeDbKey}' is null or empty.");

        return connectionString;
    }
}