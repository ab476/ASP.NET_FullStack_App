using AuthAPI.Data.Role;
using AuthAPI.Data.User;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Features.Users;


public class UserService(UserManager<TUser> userManager, RoleManager<TRole> roleManager) : IUserService
{
    private readonly UserManager<TUser> _userManager = userManager;
    private readonly RoleManager<TRole> _roleManager = roleManager;
    public async Task<OperationResult> RegisterAsync(RegisterRequest dto)
    {
        return await _userManager.CreateAsync(dto.ToTUser(), dto.Password);
    }

    public async Task<OperationResult<UserResponse[]>> GetUsersAsync()
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .ToUserResponseQueryable()
            .ToArrayAsync();

        return users;
    }

    public async Task<OperationResult> AddRoleToUserAsync(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return "User not found";

        if (!await _roleManager.RoleExistsAsync(roleName))
            await _roleManager.CreateAsync(new TRole() { Name = roleName });

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result;
    }

    public async Task<OperationResult> ResetPasswordAsync(Guid userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return "User not found";

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result;
    }

    public async Task<OperationResult> DeleteUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return "User not found";

        var result = await _userManager.DeleteAsync(user);
        return result;
    }
}
