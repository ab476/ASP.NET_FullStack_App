using AuthAPI.Services.UserAddresses;
using AuthAPI.Services.UserAddresses.Contracts;

namespace AuthAPI.Services.UserAddresses.Repository;

public class UserAddressRepository(AuthDbContext db) : IUserAddressRepository
{
    private readonly AuthDbContext _db = db;

    public async Task<UserAddressResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var address = await _db.TUserAddresses
            .TagWith("UserAddressRepository.GetByIdAsync")
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return address?.ToResponse();
    }

    public async Task<IEnumerable<UserAddressResponse>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var addresses = await _db.TUserAddresses
            .TagWith("UserAddressRepository.GetByUserIdAsync")
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);

        return addresses.Select(UserAddressMapper.ToResponse);
    }

    public async Task<Guid?> GetUserIdByAddressIdAsync(Guid addressId, CancellationToken ct)
    {
        var UserId = await _db.TUserAddresses
            .TagWith("UserAddressRepository.GetUserIdByAddressIdAsync")
            .AsNoTracking()
            .Where(x => x.Id == addressId)
            .Select(x => x.UserId)
            .FirstOrDefaultAsync(ct);

        return UserId;
    }

    public async Task<UserAddressResponse> CreateAsync(CreateUserAddressRequest request, CancellationToken ct)
    {
        var entity = request.ToEntity();

        entity.Id = Guid.NewGuid();
        entity.CreatedOn = DateTime.UtcNow;

        _db.TUserAddresses.Add(entity);
        await _db.SaveChangesAsync(ct);

        return entity.ToResponse();
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateUserAddressRequest request, CancellationToken ct)
    {
        var updatedCount = await _db.TUserAddresses
            .TagWith("UserAddressRepository.UpdateAsync.UpdateCommand")
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Street, request.Street)
                .SetProperty(x => x.City, request.City)
                .SetProperty(x => x.State, request.State)
                .SetProperty(x => x.PostalCode, request.PostalCode)
                .SetProperty(x => x.Country, request.Country)
                .SetProperty(x => x.ModifiedOn, DateTime.UtcNow),
                ct
            );                       // <-- ct passed here

        return updatedCount > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var affected = await _db.TUserAddresses
            .TagWith("UserAddressRepository.DeleteAsync")
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(ct);   // <-- ct passed here

        return affected > 0;
    }
}