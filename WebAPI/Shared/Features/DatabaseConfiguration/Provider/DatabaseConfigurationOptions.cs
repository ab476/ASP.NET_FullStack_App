namespace Common.Features.DatabaseConfiguration.Provider;

public class DatabaseConfigurationOptions
{
    public TimeSpan PollInterval { get; set; }
    public bool EnablePooling { get; set; }

    // Hook for additional DbContext options:
    public Action<DbContextOptionsBuilder>? ConfigureDbContext { get; set; }

    public const string DefaultDbScriptRoute = "/config/database-schema";
    public string? DbScriptRoute { get; set; }
}
