namespace AuthAPI.Services.Caching;

public record CachePrefixProvider(string Project, string Environment) : ICachePrefixProvider;

