namespace AuthAPI.Modules.Auth.DTOs;

public record ConfirmEmailRequest
{
    public string UserId { get; init; } = default!;
    public string Token { get; init; } = default!;
}
