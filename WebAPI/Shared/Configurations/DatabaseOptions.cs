using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Configurations;
public class DatabaseOptions
{
    public DatabaseType ActiveDatabase { get; set; }
    public Dictionary<string, string> ConnectionStrings { get; set; } = [];
}

public static partial class ServiceCollectionExtensions
{
    public static string GetActiveConnectionString(this DatabaseOptions databaseOptions)
    {
        var activeDbKey = databaseOptions.ActiveDatabase.ToString();
        databaseOptions.ConnectionStrings.TryGetValue(activeDbKey, out string? connectionString);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Connection string for '{activeDbKey}' is null or empty.");

        return connectionString;
    }
}