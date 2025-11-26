namespace AuthAPI.Modules.Auth.DTOs;

public record ConfirmPasswordResetRequest
{
    public string Email { get; init; } = default!;
    public string Token { get; init; } = default!;
    public string NewPassword { get; init; } = default!;
}