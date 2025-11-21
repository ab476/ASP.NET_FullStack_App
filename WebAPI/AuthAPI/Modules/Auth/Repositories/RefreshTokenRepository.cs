using AuthAPI.Data.Models;

namespace AuthAPI.Modules.Auth.Repositories;

public class RefreshTokenRepository(AuthDbContext _db, ITimeProvider _clock) : IRefreshTokenRepository
{
    public Task<TRefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default)
        => _db.TRefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash, ct);

    public async Task AddAsync(TRefreshToken token, CancellationToken ct = default)
    {
         await _db.TRefreshTokens.AddAsync(token, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    private async Task<bool> RevokeInternalAsync(
        IQueryable<TRefreshToken> query,
        string tag,
        string tagDetail,
        string? reason,
        CancellationToken ct)
    {
        var now = _clock.UtcNow;

        var updated = await query
            .TagWith(tag)
            .TagWith(tagDetail)
            .Where(r => !r.Revoked)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Revoked, true)
                .SetProperty(r => r.RevokedAt, now)
                .SetProperty(r => r.RevokedReason, reason),
                ct
            );

        return updated > 0;
    }

    public Task<bool> RevokeAsync(Guid refreshTokenId, string? reason = null, CancellationToken ct = default)
    {
        var query = _db.TRefreshTokens.Where(r => r.Id == refreshTokenId);
        return RevokeInternalAsync(
            query,
            "RefreshToken.RevokeById",
            $"Id={refreshTokenId}",
            reason,
            ct
        );
    }

    public Task<bool> RevokeByHashAsync(string tokenHash, string? reason = null, CancellationToken ct = default)
    {
        var query = _db.TRefreshTokens.Where(r => r.TokenHash == tokenHash);
        return RevokeInternalAsync(
            query,
            "RefreshToken.RevokeByHash",
            $"Hash={tokenHash}",
            reason,
            ct
        );
    }



    public async Task<int> RevokeAllForUserAsync(Guid userId, string reason, CancellationToken ct = default)
    {
        var now = _clock.UtcNow;

        return await _db.TRefreshTokens
            .TagWith("RefreshToken.RevokeAllForUser")
            .TagWith($"UserId={userId}")
            .Where(r => r.UserId == userId && !r.Revoked)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Revoked, true)
                .SetProperty(r => r.RevokedAt, now)
                .SetProperty(r => r.RevokedReason, reason),
                ct
            );
    }

    public Task<List<TRefreshToken>> GetActiveForUserAsync(Guid userId, CancellationToken ct = default)
        =>  _db.TRefreshTokens
        .AsNoTracking()
        .TagWith("RefreshToken.GetActiveForUser")
        .TagWith($"UserId={userId}")
        .Where(r => r.UserId == userId && !r.Revoked)
        .ToListAsync(ct);
}
