using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.DTOs;

namespace AuthAPI.Modules.Auth.Services;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(TUser user, CancellationToken ct);
    Task<LoginResponse> CreateTokensForUserAsync(TUser user, string? deviceId, string? fingerprint, CancellationToken ct);
    Task<LoginResponse?> RefreshAsync(string refreshToken, string? deviceId, string? fingerprint, CancellationToken ct);
    Task<bool> RevokeRefreshTokenAsync(string refreshTokenHash, string reason, Guid? replacedByTokenId, CancellationToken ct);
}
