// /Modules/Auth/Services/TokenService.cs
using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.DTOs;
using AuthAPI.Modules.Auth.Models;
using AuthAPI.Modules.Auth.Repositories;
using AuthAPI.Modules.Auth.Settings;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthAPI.Modules.Auth.Services;

public class TokenService(
    UserManager<TUser> userManager,
    IRefreshTokenRepository repo,
    IAccessTokenFactory accessFactory,
    IRefreshTokenFactory refreshFactory,
    IRefreshTokenValidator validator,
    IOptions<JwtSettings> jwtOptions,
    IOptions<RefreshTokenSettings> rtOptions,
    ITimeProvider clock,
    ILogger<TokenService> logger) : ITokenService


{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    private readonly RefreshTokenSettings _rtSettings = rtOptions.Value;

    // generate access token using claims + roles (parallelized)
    public async Task<string> GenerateAccessTokenAsync(TUser user, CancellationToken ct)
    {
        var claimsTask = userManager.GetClaimsAsync(user);
        var rolesTask = userManager.GetRolesAsync(user);
        await Task.WhenAll(claimsTask, rolesTask);

        var userClaims = claimsTask.Result;
        var roles = rolesTask.Result;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new("username", user.UserName ?? string.Empty),
            new(JwtClaimTypes.TenantId, user.TenantId ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(userClaims);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        return accessFactory.CreateToken(claims);
    }

    public async Task<LoginResponse> CreateTokensForUserAsync(TUser user, string? deviceId, string? fingerprint, CancellationToken ct)
    {
        var accessToken = await GenerateAccessTokenAsync(user, ct);
        var pair = refreshFactory.CreatePair();
        var rtEntity = refreshFactory.CreateEntity(user.Id, pair.TokenHash, deviceId, fingerprint);

        await repo.AddAsync(rtEntity, ct);

        return new LoginResponse(accessToken, pair.RawToken, _jwtSettings.AccessTokenMinutes * 60);
    }

    public async Task<LoginResponse?> RefreshAsync(
        string refreshTokenRaw,
        string? deviceId,
        string? fingerprint,
        CancellationToken ct)
    {
        // compute hash using factory's HMAC
        var hash = refreshFactory.ComputeHash(refreshTokenRaw);

        var validated = await validator.ValidateForRefreshAsync(hash, deviceId, fingerprint, ct);
        if (validated == null) return null;

        var (row, reused) = validated.Value;

        if (reused)
        {
            logger.LogWarning("Refresh token reuse detected for user {UserId}", row.UserId);
            return null;
        }

        var pair = refreshFactory.CreatePair();
        var newRow = refreshFactory.CreateEntity(row.UserId, pair.TokenHash, deviceId, fingerprint);
        

        await repo.AddAsync(newRow, ct);
        await repo.RevokeAsync(row.Id, "Rotated", newRow.Id, ct);
        // generate new access token
        var user = await userManager.FindByIdAsync(row.UserId.ToString());
        if (user == null) return null;

        var newAccess = await GenerateAccessTokenAsync(user, ct);


        return new LoginResponse(newAccess, pair.RawToken, _jwtSettings.AccessTokenMinutes * 60);
    }

    public Task<bool> RevokeRefreshTokenAsync(string refreshTokenHash, string reason, Guid? replacedByTokenId, CancellationToken ct)
    {
        return repo.RevokeAsync(refreshTokenHash, reason, replacedByTokenId, ct);
    }
}
