namespace AuthAPI.Modules.Auth.DTOs;

public record CreateApiKeyRequest
{
    public string Name { get; init; } = default!;

    /// <summary>
    /// Optional scopes/permissions for this API key.
    /// </summary>
    public string[]? Scopes { get; init; }
}
