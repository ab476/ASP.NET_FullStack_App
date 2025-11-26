namespace AuthAPI.Modules.Auth.DTOs;

public record ApiKeyResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Key { get; init; } = default!;    // <-- plaintext key shown only once
    public DateTime CreatedAt { get; init; }
}
