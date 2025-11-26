namespace AuthAPI.Modules.Auth.DTOs;

public record MagicLinkConsumeRequest
{
    public string UserId { get; init; } = default!;
    public string Token { get; init; } = default!;
    public string? DeviceId { get; init; }
    public string? Fingerprint { get; init; }
}
