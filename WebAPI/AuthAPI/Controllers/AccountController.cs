using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthAPI.Controllers;



[ApiController]
[Route("api/[controller]")]
public class AccountController(
    IAccountService accountService, 
    ILogger<AccountController> logger
) 
    : ControllerBase
{

    // POST: api/Account/Register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await accountService.RegisterUserAsync(model);
            if (result.Succeeded)
                return Ok(new { Message = "Registration successful. Please confirm your email." });

            return BadRequest(result.Errors.Select(e => e.Description));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during registration for email: {Email}", model.Email);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    // GET: api/Account/confirm-email
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
    {
        if (userId == Guid.Empty || string.IsNullOrEmpty(token))
            return BadRequest("Invalid email confirmation request.");

        try
        {
            var result = await accountService.ConfirmEmailAsync(userId, token);
            if (result.Succeeded)
                return Ok(new { Message = "Email confirmed successfully." });

            return BadRequest(result.Errors.Select(e => e.Description));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error confirming email for UserId: {UserId}", userId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    // POST: api/Account/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await accountService.LoginUserAsync(model);

            if (result.Succeeded)
                return Ok(new { Message = "Login successful." });

            if (result.IsNotAllowed)
                return BadRequest("Email is not confirmed yet.");

            return Unauthorized("Invalid login attempt.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for email: {Email}", model.Email);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    // GET: api/Account/profile
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return Unauthorized();

        try
        {
            var profile = await accountService.GetUserProfileByEmailAsync(email);
            return Ok(profile);
        }
        catch (ArgumentException)
        {
            return NotFound("Profile not found.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching profile for {Email}", email);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    // POST: api/Account/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await accountService.LogoutUserAsync();
            return Ok(new { Message = "Logout successful." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during logout");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    // POST: api/Account/resend-confirmation
    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendEmailConfirmation(ResendConfirmationEmailRequest model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await accountService.SendEmailConfirmationAsync(model.Email);
            return Ok(new { Message = "If the email is registered, a confirmation link has been sent." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending email confirmation to: {Email}", model.Email);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}

