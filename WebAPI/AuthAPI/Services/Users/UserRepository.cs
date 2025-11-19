using AuthAPI.Data;

namespace AuthAPI.Services.Users;

public class UserRepository(AuthDbContext db) : IUserRepository
{
    public Task<bool> ExistsAsync(Guid id, CancellationToken ct)
    {
        return db.TUsers.AnyAsync(u => u.Id == id, ct);
    }
}
