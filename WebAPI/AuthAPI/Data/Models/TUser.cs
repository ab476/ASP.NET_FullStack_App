namespace AuthAPI.Data.Models;

public class TUser : IdentityUser<Guid>
{
    public string? TenantId { get; set; }
    public string? FeatureFlagsJson { get; set; }

    public virtual ICollection<TUserAddress> Addresses { get; set; } = [];
    public virtual ICollection<TRefreshToken> RefreshTokens { get; set; } = [];
    public virtual ICollection<TSingleUseToken> SingleUseTokens { get; set; } = [];
    public virtual ICollection<TApiKey> ApiKeys { get; set; } = [];
}