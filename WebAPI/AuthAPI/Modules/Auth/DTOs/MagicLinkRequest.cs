namespace AuthAPI.Modules.Auth.DTOs;

public record MagicLinkRequest
{
    public string Email { get; init; } = default!;

    /// <summary>
    /// Where to send the user after clicking the magic link.
    /// Example: https://myapp.com/auth/magic
    /// </summary>
    public string? ReturnUrl { get; init; }
}