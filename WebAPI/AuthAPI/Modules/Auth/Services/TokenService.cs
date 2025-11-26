// /Modules/Auth/Services/TokenService.cs
using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.DTOs;
using AuthAPI.Modules.Auth.Infrastructure;
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
    IDomainEventPublisher events,
    ILogger<TokenService> logger) : ITokenService


{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    private readonly RefreshTokenSettings _rtSettings = rtOptions.Value;

    // generate access token using claims + roles (parallelized)
    public async Task<string> GenerateAccessTokenAsync(TUser user)
    {
        var claimsTask = userManager.GetClaimsAsync(user);
        var rolesTask = userManager.GetRolesAsync(user);
        await Task.WhenAll(claimsTask, rolesTask);

        var userClaims = claimsTask.Result;
        var roles = rolesTask.Result;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim("username", user.UserName ?? string.Empty),
            new Claim(JwtClaimTypes.TenantId, user.TenantId ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(userClaims);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // feature flags
        var flags = await (/* injection or service call */ Task.FromResult<IEnumerable<string>>(Array.Empty<string>()));
        // replace with featureFlagService.GetFeatureFlagsForUserAsync(user) if available
        if (flags?.Any() == true)
        {
            claims.Add(new Claim(JwtClaimTypes.FeatureFlags, string.Join(',', flags)));
        }

        return accessFactory.CreateToken(claims);
    }

    public async Task<LoginResponse> CreateTokensForUserAsync(TUser user, string? deviceId, string? fingerprint, string? ip = null, string? userAgent = null)
    {
        var accessToken = await GenerateAccessTokenAsync(user);
        var pair = refreshFactory.CreatePair();
        var rtEntity = refreshFactory.CreateEntity(user.Id, pair.TokenHash, deviceId, fingerprint);

        await repo.AddAsync(rtEntity);
        await repo.SaveChangesAsync();

        // publish event
        await events.PublishAsync("refresh_token.created", new { userId = user.Id, tokenId = rtEntity.Id });

        return new LoginResponse(accessToken, pair.RawToken, _jwtSettings.AccessTokenMinutes * 60);
    }

    public async Task<LoginResponse?> RefreshAsync(string refreshTokenRaw, string? deviceId, string? fingerprint, string? ip = null, string? userAgent = null)
    {
        // compute hash using factory's HMAC
        var hash = refreshFactory.ComputeHash(refreshTokenRaw);

        var validated = await validator.ValidateForRefreshAsync(hash, deviceId, fingerprint, ip, userAgent);
        if (validated == null) return null;

        var (row, reused) = validated.Value;

        if (reused)
        {
            // published inside validator - we already revoked all if configured
            logger.LogWarning("Refresh token reuse detected for user {UserId}", row.UserId);
            return null;
        }

        // now rotate: revoke old, add new
        row.Revoked = true;
        row.RevokedAt = clock.UtcNow;

        var pair = refreshFactory.CreatePair();
        var newRow = refreshFactory.CreateEntity(row.UserId, pair.TokenHash, deviceId, fingerprint);
        // set replaced chain (if using Guid id)
        row.ReplacedByTokenId = newRow.Id;

        await repo.AddAsync(newRow);

        // generate new access token
        var user = await userManager.FindByIdAsync(row.UserId.ToString());
        if (user == null) return null;

        var newAccess = await GenerateAccessTokenAsync(user);

        await repo.SaveChangesAsync();

        await events.PublishAsync("refresh_token.rotated", new { userId = row.UserId, oldId = row.Id, newId = newRow.Id });

        return new LoginResponse(newAccess, pair.RawToken, _jwtSettings.AccessTokenMinutes * 60);
    }

    public async Task RevokeRefreshTokenAsync(string refreshTokenHash, string? reason = null)
    {
        var row = await repo.GetByHashAsync(refreshTokenHash);
        if (row == null) return;
        await repo.RevokeAsync(row.Id, reason);
        await events.PublishAsync("refresh_token.revoked", new { userId = row.UserId, tokenId = row.Id, reason });
    }
}
