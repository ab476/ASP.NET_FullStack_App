namespace AuthAPI.Features.Users;

public interface IUserService
{
    Task<OperationResult> AddRoleToUserAsync(Guid userId, string roleName);
    Task<OperationResult> DeleteUserAsync(Guid userId);
    Task<OperationResult<UserResponse[]>> GetUsersAsync();
    Task<OperationResult> RegisterAsync(RegisterRequest dto);
    Task<OperationResult> ResetPasswordAsync(Guid userId, string newPassword);
}
