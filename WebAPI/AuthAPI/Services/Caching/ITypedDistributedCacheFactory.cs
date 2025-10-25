namespace AuthAPI.Services.Caching;

public interface ITypedDistributedCacheFactory
{
    ITypedDistributedCache<TService> GetCache<TService>()  where TService : class;
}
