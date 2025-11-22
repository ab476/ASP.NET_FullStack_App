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
    ILogger<RefreshTokenValidator> logger) : IRefreshTokenValidator
{
    private readonly RefreshTokenSettings _settings = rtOptions.Value;

    /// <summary>
    /// Validates a refresh token hash and returns (row, reusedDetected).
    /// Returns null when invalid or expired.
    /// </summary>
    public async Task<(TRefreshToken row, bool reusedDetected)?> ValidateForRefreshAsync(
        string tokenHash,
        string? deviceId,
        string? fingerprint,
        string? ip,
        string? userAgent,
        CancellationToken ct = default)
    {
        // 1. Load token row
        var row = await repo.GetByHashAsync(tokenHash, ct);
        if (row == null)
            return null;

        // 2. Detect reuse
        if (row.Revoked)
            return await HandleReuseAsync(row, tokenHash, ct);

        // 3. Check expiration
        if (!IsNotExpired(row))
            return null;

        // 4. Check device binding
        if (!DeviceMatches(row, deviceId))
            return null;

        // 5. Check fingerprint binding
        if (!FingerprintMatches(row, fingerprint))
            return null;

        // 6. Optional: IP / User-Agent binding
        if (!UserAgentMatches(row, userAgent))
            return null;

        // Passed all checks
        return (row, false);
    }

    // --------------------------
    // Internal helpers
    // --------------------------

    private bool IsNotExpired(TRefreshToken row)
        => row.ExpiresAt > clock.UtcNow;

    private bool DeviceMatches(TRefreshToken row, string? deviceId)
    {
        if (!string.IsNullOrEmpty(row.DeviceId) &&
            deviceId != null &&
            row.DeviceId != deviceId)
        {
            logger.LogWarning(
                "Device mismatch for refresh token: expected {Expected} got {Got}",
                row.DeviceId, deviceId);
            return false;
        }

        return true;
    }

    private bool FingerprintMatches(TRefreshToken row, string? fingerprint)
    {
        if (!string.IsNullOrEmpty(row.FingerprintHash) &&
            fingerprint != null &&
            row.FingerprintHash != fingerprint)
        {
            logger.LogWarning(
                "Fingerprint mismatch for refresh token for user {UserId}",
                row.UserId);
            return false;
        }

        return true;
    }

    private bool UserAgentMatches(TRefreshToken row, string? userAgent)
    {
        // Placeholder for future properties
        // e.g., row.UserAgent
        return true;
    }

    private async Task<(TRefreshToken row, bool reusedDetected)?> HandleReuseAsync(
        TRefreshToken row,
        string tokenHash,
        CancellationToken ct)
    {
        logger.LogWarning(
            "Refresh token reuse detected for user {UserId}",
            row.UserId);

        await PublishReuseEventAsync(row, tokenHash, ct);

        if (_settings.RevokeOnReuse)
        {
            await repo.RevokeAllForUserAsync(
                row.UserId,
                "Refresh token reuse detected",
                ct
            );
        }

        return (row, true);
    }

    private Task PublishReuseEventAsync(TRefreshToken row, string tokenHash, CancellationToken ct)
    {
        return events.PublishAsync(
            "refresh_token.reuse",
            new { userId = row.UserId, tokenHash },
            ct
        );
    }
}
