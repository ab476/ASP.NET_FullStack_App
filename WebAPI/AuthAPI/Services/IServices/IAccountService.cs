
namespace AuthAPI.Services.IServices;

public interface IAccountService
{
    Task<IdentityResult> RegisterUserAsync(RegisterRequest model);
    Task<IdentityResult> ConfirmEmailAsync(Guid userId, string token);
    Task<SignInResult> LoginUserAsync(LoginRequest model);
    Task LogoutUserAsync();
    Task SendEmailConfirmationAsync(string email);
    Task<ProfileResponse> GetUserProfileByEmailAsync(string email);
}

public class RegisterRequest
{
}