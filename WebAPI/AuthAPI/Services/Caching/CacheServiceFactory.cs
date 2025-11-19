namespace AuthAPI.Services.Caching;

public class CacheServiceFactory(IServiceProvider serviceProvider) : ICacheServiceFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ICacheService<TService> GetCache<TService>() where TService : class
    {
        return _serviceProvider.GetRequiredService<ICacheService<TService>>();
    }
}