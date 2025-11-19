using AuthAPI.Services.UserAddresses.Contracts;

namespace AuthAPI.Services.UserAddresses.Repository;

public interface IUserAddressRepository
{
    Task<UserAddressResponse?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<UserAddressResponse>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<UserAddressResponse> CreateAsync(CreateUserAddressRequest request, CancellationToken ct);
    Task<UserAddressResponse?> UpdateAsync(Guid id, UpdateUserAddressRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}