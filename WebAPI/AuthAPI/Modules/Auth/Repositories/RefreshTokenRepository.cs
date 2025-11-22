using AuthAPI.Data.Models;

namespace AuthAPI.Modules.Auth.Repositories;

public class RefreshTokenRepository(AuthDbContext _db, ITimeProvider _clock) : IRefreshTokenRepository
{
    // ---------- Query Helpers ----------

    private IQueryable<TRefreshToken> TRefreshTokens => _db.TRefreshTokens.AsNoTracking();
    private IQueryable<TRefreshToken> ActiveTRefreshTokens => TRefreshTokens.Where(r => !r.Revoked);

    // ---------- Getters ----------

    public Task<TRefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default)
        => TRefreshTokens
            .AsNoTracking()
            .TagWith("RefreshToken.GetByHash")
            .TagWith($"Hash={tokenHash}")
            .Where(r => r.TokenHash == tokenHash)
            .FirstOrDefaultAsync(ct);

    public Task<List<TRefreshToken>> GetActiveForUserAsync(Guid userId, CancellationToken ct = default)
        => ActiveTRefreshTokens
            .AsNoTracking()
            .TagWith("RefreshToken.GetActiveForUser")
            .TagWith($"UserId={userId}")
            .Where(r => r.UserId == userId)
            .ToListAsync(ct);

    // ---------- Add / Save ----------

    public async Task AddAsync(TRefreshToken token, CancellationToken ct = default)
    {
        await _db.TRefreshTokens.AddAsync(token, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    // ---------- Internal Revoke Helpers ----------

    private async Task<int> RevokeInternalAsync(
        IQueryable<TRefreshToken> query,
        string tag,
        string detail,
        string? reason,
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
                .SetProperty(r => r.RevokedReason, reason),
                ct
            );
    }

    // ---------- Public Revocation API ----------

    public async Task<bool> RevokeAsync(Guid refreshTokenId, string? reason = null, CancellationToken ct = default)
    {
        var q = TRefreshTokens.Where(r => r.Id == refreshTokenId);
        return await RevokeInternalAsync(q, "RefreshToken.RevokeById", $"Id={refreshTokenId}", reason, ct) > 0;
    }

    public async Task<bool> RevokeByHashAsync(string tokenHash, string? reason = null, CancellationToken ct = default)
    {
        var q = TRefreshTokens.Where(r => r.TokenHash == tokenHash);
        return await RevokeInternalAsync(q, "RefreshToken.RevokeByHash", $"Hash={tokenHash}", reason, ct) > 0;
    }

    public Task<int> RevokeAllForUserAsync(Guid userId, string reason, CancellationToken ct = default)
    {
        var q = ActiveTRefreshTokens.Where(r => r.UserId == userId);
        return RevokeInternalAsync(q, "RefreshToken.RevokeAllForUser", $"UserId={userId}", reason, ct);
    }
}
