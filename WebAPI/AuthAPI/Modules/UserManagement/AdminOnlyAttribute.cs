using AuthAPI.Helper;
using Microsoft.AspNetCore.Authorization;

namespace AuthAPI.Modules.UserManagement;

public sealed class AdminOnlyAttribute : AuthorizeAttribute
{
    public const string PolicyName = "AdminOnly";

    public AdminOnlyAttribute()
    {
        Policy = PolicyName;
    }
}

public static class AddAdminOnlyPolicyExtensions
{
    public static AuthorizationBuilder AddAdminOnlyPolicy(this AuthorizationBuilder authorizationBuilder)
    {
        authorizationBuilder
            .AddPolicy(AdminOnlyAttribute.PolicyName, policy =>
                policy.RequireRole(UserRoles.Admin));

        return authorizationBuilder;
    }

    public static RouteGroupBuilder RequireAdmin(this RouteGroupBuilder group) =>
       group.RequireAuthorization(AdminOnlyAttribute.PolicyName);
}
