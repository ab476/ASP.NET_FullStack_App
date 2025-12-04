using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.Repositories;
using AuthAPI.Modules.Auth.Settings;
using Microsoft.Extensions.Options;

namespace AuthAPI.Modules.Auth.Services;

/// <summary>
/// Validates refresh tokens, detects reuse, and enforces token binding rules.
/// </summary>
public class RefreshTokenValidator(
    IRefreshTokenRepository repo,
    IOptions<RefreshTokenSettings> rtOptions,
    ITimeProvider clock) : IRefreshTokenValidator
{
    private readonly RefreshTokenSettings _settings = rtOptions?.Value ?? throw new ArgumentNullException(nameof(rtOptions));

    /// <summary>
    /// Validates a refresh token hash and returns (row, reusedDetected).
    /// Returns null when invalid or expired.
    /// </summary>
    public async Task<(TRefreshToken row, bool reusedDetected)?> ValidateForRefreshAsync(
        string tokenHash,
        string? deviceId,
        string? fingerprint,
        CancellationToken ct = default)
    {
        // 0. Basic checks
        if (string.IsNullOrWhiteSpace(tokenHash))
            return null;

        // 1. Load token row
        var row = await repo.GetByHashAsync(tokenHash, ct);
        if (row == null)
            return null;

        // 2. Detect reuse
        if (row.Revoked)
            return await HandleReuseAsync(row, ct);

        // 3. Check expiration
        if (IsExpired(row))
            return null;

        // 4. Check device binding
        if (IsDeviceMismatch(row, deviceId))
            return null;

        // 5. Check fingerprint binding
        if (IsFingerprintMismatch(row, fingerprint))
            return null;

        // Passed all checks
        return (row, false);
    }

    // --------------------------
    // Internal helpers
    // --------------------------

    /// <summary>
    /// Checks whether the refresh token is expired.
    /// </summary>
    private bool IsExpired(TRefreshToken row)
    {
        return row.ExpiresAt > clock.UtcNow;
    }

    /// <summary>
    /// Determines whether a stored value mismatches the provided value.
    /// </summary>
    private static bool IsMismatch(string? stored, string? supplied)
    {
        // No stored binding → cannot mismatch
        if (string.IsNullOrEmpty(stored))
            return true;

        // Missing input → mismatch
        if (string.IsNullOrEmpty(supplied))
            return true;

        // Compare
        bool mismatch = stored != supplied;

        return mismatch;
    }

    /// <summary>
    /// Checks whether the device identifier mismatches.
    /// </summary>
    private static bool IsDeviceMismatch(TRefreshToken row, string? deviceId)
    {
        return IsMismatch(row.DeviceId, deviceId);
    }

    /// <summary>
    /// Checks whether the fingerprint mismatches.
    /// </summary>
    private static bool IsFingerprintMismatch(TRefreshToken row, string? fingerprint)
    {
        return IsMismatch(row.FingerprintHash, fingerprint);
    }

    /// <summary>
    /// Handles refresh token reuse and revocation.
    /// </summary>
    private async Task<(TRefreshToken row, bool reusedDetected)?> HandleReuseAsync(
        TRefreshToken row,
        CancellationToken ct)
    {
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
    
}
