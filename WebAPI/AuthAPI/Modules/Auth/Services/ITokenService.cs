using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.DTOs;

namespace AuthAPI.Modules.Auth.Services;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(TUser user);
    (string token, string hash) CreateRefreshToken();
    Task<AuthResponse> CreateTokensForUserAsync(TUser user, string? deviceId, string? fingerprint);
    Task<AuthResponse?> RefreshAsync(string refreshToken, string? deviceId, string? fingerprint);
    Task RevokeRefreshTokenAsync(string refreshTokenHash, string? reason = null);
}
