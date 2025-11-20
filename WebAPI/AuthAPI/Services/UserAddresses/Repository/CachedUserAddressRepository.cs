using AuthAPI.Services.Caching;
using AuthAPI.Services.UserAddresses.Contracts;
using static AuthAPI.Services.Caching.AuthApiCacheKeys;

namespace AuthAPI.Services.UserAddresses.Repository;

public class CachedUserAddressRepository(
    IUserAddressRepository _inner,
    ICacheService _cache
) : IUserAddressRepository
{
    private const int _ttlMinutes = 5;

    // ---------------------------------------------------------
    // READ METHODS (clean + DRY)
    // ---------------------------------------------------------

    public Task<UserAddressResponse?> GetByIdAsync(Guid id, CancellationToken ct)
        => _cache.GetOrSetAsync(
            UserAddressById(id),
            () => _inner.GetByIdAsync(id, ct),
            _ttlMinutes
        );


    public Task<Guid?> GetUserIdByAddressIdAsync(Guid addressId, CancellationToken ct)
        => _cache.GetOrSetAsync(
            UserIdByAddressId(addressId),
            () => _inner.GetUserIdByAddressIdAsync(addressId, ct),
            _ttlMinutes
        );


    public async Task<IEnumerable<UserAddressResponse>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await _cache.GetOrSetAsync(
            AddressesByUserId(userId),
            async () =>
            {
                var result = await _inner.GetByUserIdAsync(userId, ct);
                return result.ToList(); // Cache-ready list
            },
            _ttlMinutes
        ) ?? [];


    // ---------------------------------------------------------
    // WRITE METHODS (shared helpers to minimize duplication)
    // ---------------------------------------------------------

    public async Task<UserAddressResponse> CreateAsync(CreateUserAddressRequest request, CancellationToken ct)
    {
        var created = await _inner.CreateAsync(request, ct);

        await InvalidateUserAddressList(created.UserId);

        return created;
    }


    public async Task<bool> UpdateAsync(Guid id, UpdateUserAddressRequest request, CancellationToken ct)
    {
        var userId = await GetUserIdByAddressIdAsync(id, ct);
        var updated = await _inner.UpdateAsync(id, request, ct);

        if (updated && userId is not null)
            await InvalidateAllRelatedKeys(id, userId.Value);

        return updated;
    }


    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var userId = await GetUserIdByAddressIdAsync(id, ct);
        var deleted = await _inner.DeleteAsync(id, ct);

        if (deleted && userId is not null)
            await InvalidateAllRelatedKeys(id, userId.Value);

        return deleted;
    }


    // ---------------------------------------------------------
    // PRIVATE HELPERS (zero duplication)
    // ---------------------------------------------------------

    private Task InvalidateUserAddressList(Guid userId)
        => _cache.RemoveAsync(AddressesByUserId(userId));


    private async Task InvalidateAllRelatedKeys(Guid addressId, Guid userId)
    {
        await _cache.RemoveAsync(UserAddressById(addressId));
        await InvalidateUserAddressList(userId);
        await _cache.RemoveAsync(UserIdByAddressId(addressId));
    }
}