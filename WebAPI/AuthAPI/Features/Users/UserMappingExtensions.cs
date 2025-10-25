using AuthAPI.Data.User;
using Riok.Mapperly.Abstractions;

namespace AuthAPI.Features.Users;

[Mapper]
public static partial class UserMappingExtensions
{
    /// <summary>🔹 Convert RegisterRequest → TUser (e.g., ApplicationUser) </summary>
    public static partial TUser ToTUser(this RegisterRequest request);

    /// <summary>🔹 Convert IdentityUser → UserResponse </summary>
    public static partial UserResponse ToUserResponse(this TUser user);

    public static partial IQueryable<UserResponse> ToUserResponseQueryable(this IQueryable<TUser> query);
}
