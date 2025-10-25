namespace Common.Configurations;
public class MultiDatabaseConfig
{
    public DatabaseType ActiveDatabase { get; set; }
    public Dictionary<string, string> ConnectionStrings { get; set; } = [];
}

public static partial class DatabaseOptionsExtensions
{
    public static string GetActiveConnectionString(this MultiDatabaseConfig databaseOptions)
    {
        var activeDbKey = databaseOptions.ActiveDatabase.ToString();
        databaseOptions.ConnectionStrings.TryGetValue(activeDbKey, out string? connectionString);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Connection string for '{activeDbKey}' is null or empty.");

        return connectionString;
    }
}