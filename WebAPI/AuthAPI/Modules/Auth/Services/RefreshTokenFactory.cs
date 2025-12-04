using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.Models;
using AuthAPI.Modules.Auth.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace AuthAPI.Modules.Auth.Services;

/// <summary>
/// Creates refresh tokens, hashes, and token entities.
/// </summary>
public class RefreshTokenFactory(
    IOptions<JwtSettings> jwtOptions,
    IOptions<RefreshTokenSettings> rtOptions,
    ITimeProvider _clock) : IRefreshTokenFactory
{
    private readonly byte[] _hmacKey = Encoding.UTF8.GetBytes(jwtOptions.Value.RefreshSecret ?? jwtOptions.Value.Secret);
    private readonly RefreshTokenSettings _rtSettings = rtOptions.Value;

    /// <summary>
    /// Generates a raw refresh token and its HMAC hash pair.
    /// </summary>
    public RefreshTokenPair CreatePair()
    {
        // generate 512-bit random token
        var rawBytes = RandomNumberGenerator.GetBytes(64);
        var raw = Convert.ToBase64String(rawBytes);

        var hash = ComputeHash(raw);

        return new RefreshTokenPair(raw, hash);
    }

    /// <summary>
    /// Creates a refresh token database entity.
    /// </summary>
    public TRefreshToken CreateEntity(Guid userId, string tokenHash, string? deviceId, string? fingerprint)
    {
        var now = _clock.UtcNow;

        return new TRefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAt = now,
            ExpiresAt = now.AddDays(_rtSettings.LifetimeDays),
            DeviceId = deviceId,
            FingerprintHash = fingerprint,
            Revoked = false
        };
    }

    /// <summary>
    /// Computes the HMAC SHA-256 hash of a raw token.
    /// </summary>
    public string ComputeHash(string rawToken)
    {
        using var hmac = new HMACSHA256(_hmacKey);
        var rawBytes = Encoding.UTF8.GetBytes(rawToken);
        var hashBytes = hmac.ComputeHash(rawBytes);
        return Convert.ToHexString(hashBytes);
    }
}
