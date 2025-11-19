using Microsoft.AspNetCore.Authorization;

namespace AuthAPI.Helper;

/// <summary>
/// Marks an endpoint as requiring the Admin role.
/// </summary>
public class AdminAuthorizeAttribute : AuthorizeAttribute
{
    public AdminAuthorizeAttribute()
    {
        Roles = "Admin";
    }
}