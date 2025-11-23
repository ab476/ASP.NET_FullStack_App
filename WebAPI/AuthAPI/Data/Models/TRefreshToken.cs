namespace AuthAPI.Data.Models;

public class TRefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public TUser User { get; set; } = null!;

    public string TokenHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public string? DeviceId { get; set; }
    public string? FingerprintHash { get; set; }

    public bool Revoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedReason { get; set; }

    public Guid? ReplacedByTokenId { get; set; }
}
