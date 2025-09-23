namespace Shared.Options;
public class DatabaseOptions
{
    public DatabaseType ActiveDatabase { get; set; }
    public Dictionary<string, string> ConnectionStrings { get; set; } = [];
}
