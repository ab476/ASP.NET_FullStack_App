namespace AuthAPI.Modules.Auth.Models;

public static class JwtClaimTypes
{
    public const string TenantId = "tenantId";
    public const string FeatureFlags = "featureFlags";

    /// <summary>
    /// Represents the claim type for a UNIX expiration timestamp in seconds since the epoch.
    /// </summary>
    /// <remarks>This constant is typically used in token-based authentication systems to indicate the
    /// expiration time of a token as a UNIX timestamp. The value should correspond to the number of seconds elapsed
    /// since January 1, 1970 (UTC).</remarks>
    public const string ExpUnix = "exp_unix";
}
