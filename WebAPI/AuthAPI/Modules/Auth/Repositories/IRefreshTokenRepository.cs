using AuthAPI.Data.Models;

namespace AuthAPI.Modules.Auth.Repositories;

public interface IRefreshTokenRepository
{
    Task<TRefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct);
    Task AddAsync(TRefreshToken token, CancellationToken ct);
    Task<bool> RevokeAsync(Guid refreshTokenId, string reason, Guid? replacedByTokenId, CancellationToken ct);
    Task<bool> RevokeAsync(string tokenHash, string reason, Guid? replacedByTokenId, CancellationToken ct);
    Task<int> RevokeAllForUserAsync(Guid userId, string reason, CancellationToken ct);
    Task<List<TRefreshToken>> GetActiveForUserAsync(Guid userId, CancellationToken ct);
}
