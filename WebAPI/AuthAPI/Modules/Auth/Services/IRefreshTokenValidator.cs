using AuthAPI.Data.Models;

namespace AuthAPI.Modules.Auth.Services;

public interface IRefreshTokenValidator
{
    Task<(TRefreshToken row, bool reusedDetected)?> ValidateForRefreshAsync(string tokenHash, string? deviceId, string? fingerprint, string? ip, string? userAgent, CancellationToken ct = default);
}