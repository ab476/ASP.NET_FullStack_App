namespace AuthAPI.Services.Users;

public interface IUserRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct);
}

