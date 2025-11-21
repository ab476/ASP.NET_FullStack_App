namespace AuthAPI.Data.Models;

public class TApiKey
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public TUser User { get; set; } = null!;

    public string KeyHash { get; set; } = null!;
    public string? Scopes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }

    public bool Revoked { get; set; }
}
