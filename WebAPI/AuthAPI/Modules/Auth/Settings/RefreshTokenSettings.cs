namespace AuthAPI.Modules.Auth.Settings;

public class RefreshTokenSettings
{
    public int LifetimeDays { get; set; } = 30;
    public bool RotateOnUse { get; set; } = true;
    // optional: if true, revoke all tokens on suspicious reuse
    public bool RevokeOnReuse { get; set; } = true;
}
