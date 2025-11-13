using MemoryPack;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthAPI.Services.Caching;

public class TypedDistributedCache<TService>(
    IDistributedCache cache,
    ILogger<TypedDistributedCache<TService>> logger
) : ITypedDistributedCache<TService> 
    where TService : class
{
    private readonly IDistributedCache _cache = cache;
    private readonly ILogger<TypedDistributedCache<TService>> _logger = logger;

    private readonly string _FullName = typeof(TService).FullName!;

    public async Task<(bool Found, TValue? Value)> TryGetValueAsync<TValue>(string key)
    {
        var value = await GetAsync<TValue>(key);
        return (value is not null, value);
    }

    public async Task<TValue?> GetAsync<TValue>(string key)
    {
        var data = await _cache.GetAsync(BuildKey(key));
        return data is null ? default : Deserialize<TValue>(data);
    }

    public async Task SetAsync<TValue>(string key, TValue item, int ttlMinutes)
    {
        if (item is null)
        {
            _logger.LogWarning("Attempted to cache null value for key {Key}.", key);
            return;
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes)
        };

        await _cache.SetAsync(BuildKey(key), Serialize(item), options);
    }

    public Task RemoveAsync(string key) =>
        _cache.RemoveAsync(BuildKey(key));

    private string BuildKey(string key) => $"{_FullName}_{key}";

    private byte[] Serialize<TValue>(TValue item)
    {
        try
        {
            return MemoryPackSerializer.Serialize(item);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"MemoryPack serialization failed for '{_FullName}'.", ex);
        }
    }

    private TValue? Deserialize<TValue>(byte[] data)
    {
        try
        {
            return MemoryPackSerializer.Deserialize<TValue>(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "MemoryPack deserialization failed for type {Type}.",
                _FullName);
            return default;
        }
    }
}
