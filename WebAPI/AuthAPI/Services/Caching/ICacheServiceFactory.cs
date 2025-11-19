namespace AuthAPI.Services.Caching;

public interface ICacheServiceFactory
{
    ICacheService<TService> GetCache<TService>() where TService : class;
}