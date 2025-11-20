
namespace AuthAPI.Services.Caching;

public interface ICacheService
{
    Task<TValue?> GetAsync<TValue>(string key);
    Task RemoveAsync(string key);
    Task SetAsync<TValue>(string key, TValue item, int ttlMinutes);
    Task<(bool Found, TValue? Value)> TryGetValueAsync<TValue>(string key);
    Task<TValue?> GetOrSetAsync<TValue>(string key, Func<Task<TValue?>> factory, int ttlMinutes);

}