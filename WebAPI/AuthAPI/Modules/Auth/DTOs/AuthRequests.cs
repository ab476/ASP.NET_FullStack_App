namespace AuthAPI.Modules.Auth.DTOs;

public record LoginRequest(string Email, string Password, string? DeviceId, string? Fingerprint);
public record RefreshRequest(string RefreshToken, string? DeviceId, string? Fingerprint);
public record RegisterRequest(string Email, string Password, string? Username, string? TenantId);