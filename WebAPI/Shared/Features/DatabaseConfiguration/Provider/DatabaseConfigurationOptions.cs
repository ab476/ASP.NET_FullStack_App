namespace Common.Features.DatabaseConfiguration.Provider;

public class DatabaseConfigurationOptions
{
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnablePooling { get; set; } = true;

    // Hook for additional DbContext options:
    public Action<DbContextOptionsBuilder>? ConfigureDbContext { get; set; }
}
