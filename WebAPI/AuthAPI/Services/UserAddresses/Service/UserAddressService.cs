using AuthAPI.Services.UserAddresses.Contracts;
using AuthAPI.Services.UserAddresses.Repository;

namespace AuthAPI.Services.UserAddresses.Service;

public class UserAddressService(IUserAddressRepository _repo) : IUserAddressService
{

    public Task<UserAddressResponse?> GetByIdAsync(Guid id, CancellationToken ct) => 
        _repo.GetByIdAsync(id, ct);

    public Task<Guid?> GetUserIdByAddressIdAsync(Guid addressId, CancellationToken ct) =>
        _repo.GetUserIdByAddressIdAsync(addressId, ct);

    public Task<IEnumerable<UserAddressResponse>> GetByUserIdAsync(Guid userId, CancellationToken ct) => 
        _repo.GetByUserIdAsync(userId, ct);

    public Task<UserAddressResponse> CreateAsync(CreateUserAddressRequest req, CancellationToken ct) => 
        _repo.CreateAsync(req, ct);

    public Task<bool> UpdateAsync(Guid id, UpdateUserAddressRequest req, CancellationToken ct) => 
        _repo.UpdateAsync(id, req, ct);

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => 
        _repo.DeleteAsync(id, ct);
    
}