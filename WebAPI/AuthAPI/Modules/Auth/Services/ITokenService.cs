using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.DTOs;

namespace AuthAPI.Modules.Auth.Services;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(TUser user);
    Task<LoginResponse> CreateTokensForUserAsync(TUser user, string? deviceId, string? fingerprint, string? ip = null, string? userAgent = null);
    Task<LoginResponse?> RefreshAsync(string refreshToken, string? deviceId, string? fingerprint, string? ip = null, string? userAgent = null);
    Task RevokeRefreshTokenAsync(string refreshTokenHash, string? reason = null);
}
