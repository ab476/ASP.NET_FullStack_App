namespace AuthAPI.Services.Caching;

public interface ICachePrefixProvider
{
    string Project { get; }
    string Environment { get; }
}

