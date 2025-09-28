using AuthAPI.Helper;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AuthAPI.Services;


public class AccountService(
    UserManager<TUser> userManager,
    SignInManager<TUser> signInManager,
    IEmailService emailService,
    IConfiguration configuration
) 
    : IAccountService
{
    public async Task<IdentityResult> RegisterUserAsync(RegisterRequest registerRequest)
    {
        TUser user = registerRequest.MapToTUser();

        IdentityResult result = await userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded)
            return result;

        // Assign "User" role by default
        IdentityResult roleAssignResult = await userManager.AddToRoleAsync(user, "User");
        if (!roleAssignResult.Succeeded)
        {
            // Handle error - optionally return this failure instead
            // or log the issue and continue
            return roleAssignResult;
        }

        var token = await GenerateEmailConfirmationTokenAsync(user);

        var baseUrl = configuration["AppSettings:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is not configured.");
        var confirmationLink = $"{baseUrl}/Account/ConfirmEmail?userId={user.Id}&token={token}";

        await emailService.SendRegistrationConfirmationEmailAsync(user.Email, user.FirstName, confirmationLink);

        return result;
    }

    public async Task<IdentityResult> ConfirmEmailAsync(Guid userId, string token)
    {
        if (userId == Guid.Empty || string.IsNullOrEmpty(token))
            return IdentityResult.Failed(new IdentityError { Description = "Invalid token or user ID." });

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        var decodedBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedBytes);

        var result = await userManager.ConfirmEmailAsync(user, decodedToken);

        if (result.Succeeded)
        {
            var baseUrl = configuration["AppSettings:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is not configured.");
            var loginLink = $"{baseUrl}/Account/Login";

            await emailService.SendAccountCreatedEmailAsync(user.Email!, user.FirstName!, loginLink);
        }

        return result;
    }

    public async Task<SignInResult> LoginUserAsync(LoginRequest model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user == null)
            return SignInResult.Failed;

        if (!await userManager.IsEmailConfirmedAsync(user))
            return SignInResult.NotAllowed;

        var result = await signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // Update LastLogin
            user.LastLogin = DateTime.UtcNow;
            await userManager.UpdateAsync(user);
        }

        return result;
    }

    public async Task LogoutUserAsync()
    {
        await signInManager.SignOutAsync();
    }

    public async Task SendEmailConfirmationAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Prevent user enumeration by not disclosing existence
            return;
        }

        if (await userManager.IsEmailConfirmedAsync(user))
        {
            // Email already confirmed; no action needed
            return;
        }

        var token = await GenerateEmailConfirmationTokenAsync(user);

        var baseUrl = configuration["AppSettings:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is not configured.");
        var confirmationLink = $"{baseUrl}/Account/ConfirmEmail?userId={user.Id}&token={token}";

        await emailService.SendResendConfirmationEmailAsync(user.Email!, user.FirstName!, confirmationLink);
    }

    public async Task<ProfileResponse> GetUserProfileByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));

        var user = await userManager.FindByEmailAsync(email) 
            ?? throw new ArgumentException("User not found.", nameof(email));
        return user.MapToProfileResponse();
    }

    //Helper Method
    private async Task<string> GenerateEmailConfirmationTokenAsync(TUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        return encodedToken;
    }
}
