namespace Common.Features.DatabaseConfiguration.Data;

public class ConfigurationEntry
{
    /// <summary>
    /// Unique configuration key (primary identifier).
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// The stored configuration value (typically JSON or text).
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    /// Timestamp of last update. Defaults to CURRENT_TIMESTAMP.
    /// </summary>
    public DateTimeOffset LastUpdated { get; set; }

    /// <summary>
    /// For optimistic concurrency control.
    /// </summary>
    public byte[] RowVersion { get; set; } = default!;
}

