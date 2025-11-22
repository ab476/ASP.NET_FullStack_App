using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.Models;
using AuthAPI.Modules.Auth.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace AuthAPI.Modules.Auth.Services;

public class RefreshTokenFactory(
    IOptions<JwtSettings> jwtOptions,
    IOptions<RefreshTokenSettings> rtOptions,
    ITimeProvider _clock) : IRefreshTokenFactory
{
    private readonly byte[] _hmacKey = Encoding.UTF8.GetBytes(jwtOptions.Value.RefreshSecret ?? jwtOptions.Value.Secret);
    private readonly RefreshTokenSettings _rtSettings = rtOptions.Value;

    public RefreshTokenPair CreatePair()
    {
        // generate 512-bit random token
        var rawBytes = RandomNumberGenerator.GetBytes(64);
        var raw = Convert.ToBase64String(rawBytes);

        var hash = ComputeHash(raw);

        return new RefreshTokenPair(raw, hash);
    }

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

    public string ComputeHash(string rawToken)
    {
        using var hmac = new HMACSHA256(_hmacKey);
        var rawBytes = Encoding.UTF8.GetBytes(rawToken);
        var hashBytes = hmac.ComputeHash(rawBytes);
        return Convert.ToHexString(hashBytes);
    }
}
