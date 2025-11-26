namespace AuthAPI.Modules.Auth.DTOs;

public record PasswordResetRequest
{
    public string Email { get; init; } = default!;

    /// <summary>
    /// Optional URL you want to append the reset token & email to.
    /// Example: https://myapp.com/reset-password
    /// </summary>
    public string? SendToUrl { get; init; }
}
