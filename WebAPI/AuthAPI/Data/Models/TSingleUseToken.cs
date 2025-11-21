namespace AuthAPI.Data.Models;

public class TSingleUseToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public TUser User { get; set; } = null!;

    public required string TokenHash { get; set; }
    public required string Purpose { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public bool Used { get; set; }
    public DateTime? UsedAt { get; set; }

}
