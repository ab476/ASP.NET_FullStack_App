namespace AuthAPI.Services.Caching;

public class TypedDistributedCacheFactory(IServiceProvider serviceProvider) : ITypedDistributedCacheFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITypedDistributedCache<TService> GetCache<TService>() where TService : class
    {
        return _serviceProvider.GetRequiredService<ITypedDistributedCache<TService>>();
    }
}

