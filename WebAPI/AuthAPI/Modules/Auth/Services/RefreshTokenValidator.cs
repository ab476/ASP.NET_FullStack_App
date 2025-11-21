// /Modules/Auth/Services/RefreshTokenValidator.cs
using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.Infrastructure;
using AuthAPI.Modules.Auth.Repositories;
using AuthAPI.Modules.Auth.Settings;
using Microsoft.Extensions.Options;

namespace AuthAPI.Modules.Auth.Services;

public class RefreshTokenValidator(
    IRefreshTokenRepository repo,
    IDomainEventPublisher events,
    IOptions<RefreshTokenSettings> rtOptions,
    ITimeProvider clock,
    ILogger<RefreshTokenValidator> logger)
{
    private readonly RefreshTokenSettings _rtSettings = rtOptions.Value;

    public async Task<(TRefreshToken row, bool reusedDetected)?> ValidateForRefreshAsync(string hash, string? deviceId, string? fingerprint, string? ip, string? userAgent, CancellationToken ct = default)
    {
        var row = await repo.GetByHashAsync(hash, ct);
        if (row == null) return null;

        // If revoked -> treat as reuse attempt. Revoke all optionally, publish event.
        if (row.Revoked)
        {
            logger.LogWarning("Refresh token reuse detected for user {UserId}", row.UserId);
            if (_rtSettings.RevokeOnReuse)
            {
                await repo.RevokeAllForUserAsync(row.UserId, "Refresh token reuse detected", ct);
            }
            await events.PublishAsync("refresh_token.reuse", new { userId = row.UserId, tokenHash = hash }, ct);
            return (row, true);
        }

        // expired check
        if (row.ExpiresAt < clock.UtcNow) return null;

        // device binding
        if (!string.IsNullOrEmpty(row.DeviceId) && deviceId != null && row.DeviceId != deviceId)
        {
            logger.LogWarning("Device mismatch for refresh token: expected {Expected} got {Got}", row.DeviceId, deviceId);
            return null;
        }

        // fingerprint binding (if stored)
        if (!string.IsNullOrEmpty(row.FingerprintHash) && fingerprint != null && row.FingerprintHash != fingerprint)
        {
            logger.LogWarning("Fingerprint mismatch for refresh token for user {UserId}", row.UserId);
            return null;
        }

        // optional: ip / user-agent binding checks (if stored previously)
        // e.g., if row has an IpAddress property; demonstrate pattern:
        // if (!string.IsNullOrEmpty(row.IpAddress) && ip != null && row.IpAddress != ip) return null;

        return (row, false);
    }
}
