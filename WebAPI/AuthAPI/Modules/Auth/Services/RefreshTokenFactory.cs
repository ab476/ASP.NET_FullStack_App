// /Modules/Auth/Services/RefreshTokenFactory.cs
using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.Models;
using AuthAPI.Modules.Auth.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace AuthAPI.Modules.Auth.Services;

public class RefreshTokenFactory(
    IOptions<JwtSettings> jwtOptions,
    IOptions<RefreshTokenSettings> rtOptions,
    ITimeProvider _clock
) : IRefreshTokenFactory
{
    private readonly string _hmacSecret = jwtOptions.Value.RefreshSecret ?? jwtOptions.Value.Secret;
    private readonly RefreshTokenSettings _rtSettings = rtOptions.Value;

    public RefreshTokenPair CreatePair()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var raw = Convert.ToBase64String(bytes);

        var hash = ComputeHmacHex(raw, _hmacSecret);
        return new RefreshTokenPair(raw, hash);
    }

    public TRefreshToken CreateEntity(Guid userId, string tokenHash, string? deviceId, string? fingerprint)
    {
        return new TRefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAt = _clock.UtcNow,
            ExpiresAt = _clock.UtcNow.AddDays(_rtSettings.LifetimeDays),
            DeviceId = deviceId,
            FingerprintHash = fingerprint,
            Revoked = false
        };
    }

    private static string ComputeHmacHex(string value, string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash);
    }

    // expose helper to compute hash for incoming token strings
    public string ComputeHashForReceived(string rawToken) => ComputeHmacHex(rawToken, _hmacSecret);
}
