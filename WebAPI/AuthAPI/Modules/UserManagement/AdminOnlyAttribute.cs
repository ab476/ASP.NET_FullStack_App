using AuthAPI.Helper;
using Microsoft.AspNetCore.Authorization;

namespace AuthAPI.Modules.UserManagement;

/// <summary>
/// Restricts access to Admin users only.
/// </summary>
public sealed class AdminOnlyAttribute : AuthorizeAttribute
{
    public const string PolicyName = "Policy.Admin.Only";

    public AdminOnlyAttribute()
    {
        Policy = PolicyName ?? throw new InvalidOperationException("Policy name cannot be null.");
    }
}

public static class AddAdminOnlyPolicyExtensions
{
    public static AuthorizationBuilder AddAdminOnlyPolicy(this AuthorizationBuilder authorizationBuilder)
    {
        authorizationBuilder
            .AddPolicy(AdminOnlyAttribute.PolicyName, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(UserRoles.Admin);
            });

        return authorizationBuilder;
    }

    public static RouteGroupBuilder RequireAdmin(this RouteGroupBuilder group)
    {
        return group.RequireAuthorization(AdminOnlyAttribute.PolicyName);
    }
}
