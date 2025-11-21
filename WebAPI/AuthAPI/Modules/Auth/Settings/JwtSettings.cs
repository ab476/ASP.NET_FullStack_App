namespace AuthAPI.Modules.Auth.Settings;

public class JwtSettings
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Secret { get; set; } = null!;
    public int AccessTokenMinutes { get; set; } = 15;
    // optional: separate secret for refresh HMAC; if omitted we use Jwt secret
    public string? RefreshSecret { get; set; }
}