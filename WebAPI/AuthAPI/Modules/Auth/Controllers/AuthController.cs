using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.DTOs;
using AuthAPI.Modules.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthAPI.Modules.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<TUser> userManager,
    SignInManager<TUser> signInManager,
    ITokenService tokenService
) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var user = new TUser { Email = req.Email, UserName = req.Username ?? req.Email, TenantId = req.TenantId };
        var res = await userManager.CreateAsync(user, req.Password);
        if (!res.Succeeded) return BadRequest(res.Errors);

        // optionally send email confirmation using Identity token
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        // send email with token (Url encode)

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        if (user == null) return Unauthorized();

        var signinResult = await signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!signinResult.Succeeded) return Unauthorized();

        var response = await tokenService.CreateTokensForUserAsync(user, req.DeviceId, req.Fingerprint);

        // set refresh token in secure HttpOnly cookie (recommended for web)
        Response.Cookies.Append(
            "refresh_token",
            response.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            }
        );

        return Ok(response with { RefreshToken = "(in cookie)" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        // If token stored in cookie, read from cookie instead
        var rt = req.RefreshToken ?? Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(rt)) return BadRequest("No refresh token provided");

        var res = await tokenService.RefreshAsync(rt, req.DeviceId, req.Fingerprint);
        if (res == null) return Unauthorized();

        // update cookie
        Response.Cookies.Append("refresh_token", res.RefreshToken, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTimeOffset.UtcNow.AddDays(30) });

        return Ok(res with { RefreshToken = "(in cookie)" });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId == null) return Unauthorized();

        // revoke current refresh token (if provided in cookie)
        var cookie = Request.Cookies["refresh_token"];
        if (!string.IsNullOrEmpty(cookie))
        {
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cookie)));
            await tokenService.RevokeRefreshTokenAsync(hash, "logout");
        }

        // clear cookie
        Response.Cookies.Delete("refresh_token");
        return Ok();
    }

    // Additional endpoints: request password reset, confirm password reset, confirm email, magic link generation and consume, create API key
}
