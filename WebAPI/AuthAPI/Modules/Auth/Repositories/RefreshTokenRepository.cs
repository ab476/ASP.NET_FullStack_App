using AuthAPI.Data.Models;

namespace AuthAPI.Modules.Auth.Repositories;

public class RefreshTokenRepository(AuthDbContext _db, ITimeProvider _clock) : IRefreshTokenRepository
{
    // ---------- Query Helpers ----------

    private IQueryable<TRefreshToken> TRefreshTokens => _db.TRefreshTokens.AsNoTracking();
    private IQueryable<TRefreshToken> ActiveTRefreshTokens => TRefreshTokens.Where(r => !r.Revoked);

    // ---------- Getters ----------

    public Task<TRefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct)
        => TRefreshTokens
            .TagWith("RefreshToken.GetByHash")
            .TagWith($"Hash={tokenHash}")
            .Where(r => r.TokenHash == tokenHash)
            .FirstOrDefaultAsync(ct);

    public Task<List<TRefreshToken>> GetActiveForUserAsync(Guid userId, CancellationToken ct)
        => ActiveTRefreshTokens
            .TagWith("RefreshToken.GetActiveForUser")
            .TagWith($"UserId={userId}")
            .Where(r => r.UserId == userId)
            .ToListAsync(ct);

    // ---------- Add / Save ----------

    public async Task AddAsync(TRefreshToken token, CancellationToken ct)
    {
        await _db.TRefreshTokens.AddAsync(token, ct);
        await _db.SaveChangesAsync(ct);
    }

    // ---------- Internal Revoke Helpers ----------

    private async Task<int> RevokeInternalAsync(
        IQueryable<TRefreshToken> query,
        string tag,
        string detail,
        string? reason,
        Guid? replacedByTokenId,
        CancellationToken ct)
    {
        var now = _clock.UtcNow;

        return await query
            .TagWith(tag)
            .TagWith(detail)
            .Where(r => !r.Revoked)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Revoked, true)
                .SetProperty(r => r.RevokedAt, now)
                .SetProperty(r => r.RevokedReason, reason)
                .SetProperty(r => r.ReplacedByTokenId, replacedByTokenId),
                ct
            );
    }

    // ---------- Public Revocation API ----------

    public async Task<bool> RevokeAsync(Guid refreshTokenId, string reason, Guid? replacedByTokenId, CancellationToken ct)
    {
        var q = TRefreshTokens.Where(r => r.Id == refreshTokenId);
        return await RevokeInternalAsync(q, "RefreshToken.RevokeById", $"Id={refreshTokenId}", reason, replacedByTokenId, ct) > 0;
    }

    public async Task<bool> RevokeAsync(string tokenHash, string reason, Guid? replacedByTokenId, CancellationToken ct)
    {
        var q = TRefreshTokens.Where(r => r.TokenHash == tokenHash);
        return await RevokeInternalAsync(q, "RefreshToken.RevokeByHash", $"Hash={tokenHash}", reason, replacedByTokenId, ct) > 0;
    }

    public Task<int> RevokeAllForUserAsync(Guid userId, string reason, CancellationToken ct)
    {
        var query = ActiveTRefreshTokens.Where(r => r.UserId == userId);
        return RevokeInternalAsync(query, "RefreshToken.RevokeAllForUser", $"UserId={userId}", reason, null, ct);
    }
}
