using AuthAPI.Services.Caching;
using AuthAPI.Services.UserAddresses.Contracts;
using static AuthAPI.Services.Caching.AuthApiCacheKeys;

namespace AuthAPI.Services.UserAddresses.Repository;

public class CachedUserAddressRepository(
    IUserAddressRepository _inner,
    ICacheService _cache
) : IUserAddressRepository
{
    private readonly int _ttlMinutes = 5;
    
    // ---------------------------------------------
    // READ METHODS (fully optimized via GetOrSetAsync)
    // ---------------------------------------------

    public Task<UserAddressResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var key = UserAddressById(id);
        return _cache.GetOrSetAsync(
            key,
            () => _inner.GetByIdAsync(id, ct),
            _ttlMinutes
        );
    }

    public Task<Guid?> GetUserIdByAddressIdAsync(Guid addressId, CancellationToken ct)
    {
        var key = UserIdByAddressId(addressId);
        return _cache.GetOrSetAsync(
            key,
            () => _inner.GetUserIdByAddressIdAsync(addressId, ct),
            _ttlMinutes
        );
    }

    public async Task<IEnumerable<UserAddressResponse>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var key = AddressesByUserId(userId);

        return await _cache.GetOrSetAsync(
            key,
            async () =>
            {
                var result = await _inner.GetByUserIdAsync(userId, ct);
                return result.ToList(); // cache materialized list
            },
            _ttlMinutes
        ) ?? [];
    }

    // ---------------------------------------------
    // WRITE METHODS (invalidate related cache keys)
    // ---------------------------------------------

    public async Task<UserAddressResponse> CreateAsync(CreateUserAddressRequest request, CancellationToken ct)
    {
        var created = await _inner.CreateAsync(request, ct);

        // Invalidate "GetByUserId" cache
        await _cache.RemoveAsync(AddressesByUserId(created.UserId));

        return created;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateUserAddressRequest request, CancellationToken ct)
    {
        var userId = await GetUserIdByAddressIdAsync(id, ct);
        var updated = await _inner.UpdateAsync(id, request, ct);

        if (updated && userId is not null)
        {
            await _cache.RemoveAsync(UserAddressById(id));
            await _cache.RemoveAsync(AddressesByUserId(userId.Value));
            await _cache.RemoveAsync(UserIdByAddressId(id)); // new lookup invalidation
        }

        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var userId = await GetUserIdByAddressIdAsync(id, ct);
        var deleted = await _inner.DeleteAsync(id, ct);

        if (deleted && userId is not null)
        {
            await _cache.RemoveAsync(UserAddressById(id));
            await _cache.RemoveAsync(AddressesByUserId(userId.Value));
            await _cache.RemoveAsync(UserIdByAddressId(id));
        }

        return deleted;
    }
}
