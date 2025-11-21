using AuthAPI.Data.Models;

namespace AuthAPI.Modules.Auth.Repositories;

public interface IRefreshTokenRepository
{
    Task<TRefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default);
    Task AddAsync(TRefreshToken token, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<bool> RevokeAsync(Guid refreshTokenId, string? reason = null, CancellationToken ct = default);
    Task<bool> RevokeByHashAsync(string tokenHash, string? reason = null, CancellationToken ct = default);
    Task<int> RevokeAllForUserAsync(Guid userId, string reason, CancellationToken ct = default);
    Task<List<TRefreshToken>> GetActiveForUserAsync(Guid userId, CancellationToken ct = default);
}
