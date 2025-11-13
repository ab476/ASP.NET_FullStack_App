namespace Common.Features.DatabaseConfiguration.Data;
public class TConfigurationEntry
{
    public required string Key { get; set; } 
    public required string Value { get; set; } 
    public DateTimeOffset LastUpdated { get; set; }
}

