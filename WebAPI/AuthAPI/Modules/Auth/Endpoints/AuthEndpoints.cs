// AuthEndpoints.cs
using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.DTOs;
using AuthAPI.Modules.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthAPI.Modules.Auth.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/auth")
            .WithTags("Authentication");

        // ---------------- REGISTER ----------------
        group.MapPost("/register",
            async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>>>
            (
                RegisterRequest req,
                UserManager<TUser> userManager) =>
            {
                var user = new TUser
                {
                    Email = req.Email,
                    UserName = req.Username ?? req.Email,
                    TenantId = req.TenantId
                };

                var result = await userManager.CreateAsync(user, req.Password);
                if (!result.Succeeded)
                    return TypedResults.BadRequest(result.Errors);

                var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                // TODO: send email with confirmToken (Url-encoded)

                return TypedResults.Ok();
            })
            .WithSummary("Register new user")
            .WithDescription("Creates a new user account and optionally generates an email confirmation token.");

        // ---------------- LOGIN ----------------
        group.MapPost("/login",
            async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>>
            (
                HttpContext http,
                LoginRequest req,
                UserManager<TUser> userManager,
                SignInManager<TUser> signInManager,
                ITokenService tokenService,
                CancellationToken ct) =>
            {
                var user = await userManager.FindByEmailAsync(req.Email);
                if (user == null)
                    return TypedResults.Unauthorized();

                var signIn = await signInManager.CheckPasswordSignInAsync(user, req.Password, false);
                if (!signIn.Succeeded)
                    return TypedResults.Unauthorized();

                var tokens = await tokenService.CreateTokensForUserAsync(user, req.DeviceId, req.Fingerprint, ct);

                http.Response.Cookies.Append("refresh_token", tokens.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });

                return TypedResults.Ok(tokens with { RefreshToken = "(in cookie)" });
            })
            .WithSummary("Login")
            .WithDescription("Authenticates user and issues JWT + refresh token stored in HttpOnly cookie.");

        // ---------------- REFRESH TOKEN ----------------
        group.MapPost("/refresh",
            async Task<Results<Ok<LoginResponse>, BadRequest<string>, UnauthorizedHttpResult>>
            (
                HttpContext http,
                RefreshRequest req,
                ITokenService tokenService,
                CancellationToken ct) =>
            {
                var rt = req.RefreshToken ?? http.Request.Cookies["refresh_token"];
                if (string.IsNullOrEmpty(rt))
                    return TypedResults.BadRequest("No refresh token provided");

                var result = await tokenService.RefreshAsync(rt, req.DeviceId, req.Fingerprint, ct);
                if (result == null)
                    return TypedResults.Unauthorized();

                http.Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });

                return TypedResults.Ok(result with { RefreshToken = "(in cookie)" });
            })
            .WithSummary("Refresh access token")
            .WithDescription("Exchanges refresh token for new access and refresh tokens.");

        // ---------------- LOGOUT ----------------
        group.MapPost("/logout",
            [Authorize] async Task<Results<Ok, UnauthorizedHttpResult>>
            (HttpContext http,
             ClaimsPrincipal user,
             ITokenService tokenService,
             CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
                if (userId == null)
                    return TypedResults.Unauthorized();

                var cookie = http.Request.Cookies["refresh_token"];
                if (!string.IsNullOrEmpty(cookie))
                {
                    var hash = Convert.ToHexString(
                        SHA256.HashData(Encoding.UTF8.GetBytes(cookie))
                    );

                    await tokenService.RevokeRefreshTokenAsync(hash, "logout", null, ct);
                }

                http.Response.Cookies.Delete("refresh_token");
                return TypedResults.Ok();
            })
            .WithSummary("Logout")
            .WithDescription("Revokes refresh token and clears cookie.");

        // ---------------- REQUEST PASSWORD RESET ----------------
        group.MapPost("/password/request-reset",
            async Task<Results<Ok, NotFound, BadRequest<string>>>
            (PasswordResetRequest req,
             UserManager<TUser> userManager,
             IEmailSender<TUser> emailSender) =>
            {
                var user = await userManager.FindByEmailAsync(req.Email);
                if (user == null) return TypedResults.NotFound();

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var callback = string.IsNullOrEmpty(req.SendToUrl)
                    ? $"https://example.com/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}"
                    : $"{req.SendToUrl.TrimEnd('/')}?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

                // send email (implementation depends on IEmailSender)
               // await emailSender.SendPasswordResetAsync(user.Email, callback);
                
                return TypedResults.Ok();
            })
            .WithSummary("Request password reset")
            .WithDescription("Generates a password reset token and emails the user a magic link (or a URL).");

        // ---------------- CONFIRM PASSWORD RESET ----------------
        group.MapPost("/password/confirm-reset",
            async Task<Results<Ok, BadRequest<string>, NotFound>>
            (ConfirmPasswordResetRequest req,
             UserManager<TUser> userManager) =>
            {
                var user = await userManager.FindByEmailAsync(req.Email);
                if (user == null) return TypedResults.NotFound();

                var resetResult = await userManager.ResetPasswordAsync(user, req.Token, req.NewPassword);
                if (!resetResult.Succeeded)
                    return TypedResults.BadRequest(string.Join("; ", resetResult.Errors.Select(e => e.Description)));

                return TypedResults.Ok();
            })
            .WithSummary("Confirm password reset")
            .WithDescription("Consumes the reset token and sets a new password.");

        // ---------------- CONFIRM EMAIL ----------------
        group.MapPost("/email/confirm",
            async Task<Results<Ok, BadRequest<string>, NotFound>>
            (ConfirmEmailRequest req,
             UserManager<TUser> userManager) =>
            {
                var user = await userManager.FindByIdAsync(req.UserId);
                if (user == null) return TypedResults.NotFound();

                var res = await userManager.ConfirmEmailAsync(user, req.Token);
                if (!res.Succeeded)
                    return TypedResults.BadRequest(string.Join("; ", res.Errors.Select(e => e.Description)));

                return TypedResults.Ok();
            })
            .WithSummary("Confirm email")
            .WithDescription("Verifies the email confirmation token and marks the email as confirmed.");

        // ---------------- GENERATE MAGIC LINK ----------------
        group.MapPost("/magic/generate",
            async Task<Results<Ok, NotFound, BadRequest<string>>>
            (
                MagicLinkRequest req,
                UserManager<TUser> userManager,
                IEmailSender<TUser> emailSender) =>
            {
                var user = await userManager.FindByEmailAsync(req.Email);
                if (user == null) return TypedResults.NotFound();

                // Purpose "MagicLink" is arbitrary but consistent for generation/validation
                var token = await userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "MagicLink");

                var callback = string.IsNullOrEmpty(req.ReturnUrl)
                    ? $"https://example.com/magic?userId={Uri.EscapeDataString(user.Id.ToString())}&token={Uri.EscapeDataString(token)}"
                    : $"{req.ReturnUrl.TrimEnd('/')}?userId={Uri.EscapeDataString(user.Id.ToString())}&token={Uri.EscapeDataString(token)}";

                // send email with the link
                //await emailSender.SendMagicLinkAsync(user.Email, callback);
                return TypedResults.BadRequest("Not implemented: emailSender logic missing");
                return TypedResults.Ok();
            })
            .WithSummary("Generate magic link")
            .WithDescription("Generates a short-lived magic link token and emails it to the user for passwordless sign-in.");

        // ---------------- CONSUME MAGIC LINK ----------------
        group.MapPost("/magic/consume",
            async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult, NotFound>>
            (
                MagicLinkConsumeRequest req,
                UserManager<TUser> userManager,
                SignInManager<TUser> signInManager,
                ITokenService tokenService, CancellationToken ct) =>
            {
                var user = await userManager.FindByIdAsync(req.UserId);
                if (user == null) return TypedResults.NotFound();

                var valid = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "MagicLink", req.Token);
                if (!valid) return TypedResults.Unauthorized();

                // optionally: perform any checks (is email confirmed, tenant checks, etc)
                // sign in user (or skip persistent sign-in and return tokens)
                var tokens = await tokenService.CreateTokensForUserAsync(user, req.DeviceId, req.Fingerprint, ct);

                return TypedResults.Ok(tokens with { RefreshToken = "(in cookie)" });
            })
            .WithSummary("Consume magic link")
            .WithDescription("Validates a magic link token and returns access/refresh tokens for the user.");

        // ---------------- CREATE API KEY ----------------
        // This endpoint returns the plaintext API key only at creation time.
        // The server should store a hashed representation (sha256) and never return it again.
        group.MapPost("/apikeys",
            [Authorize] async Task<Results<Created<ApiKeyResponse>, BadRequest<string>, UnauthorizedHttpResult>>
            (   CreateApiKeyRequest req,
                ClaimsPrincipal user,
                UserManager<TUser> userManager,
                ITokenService tokenService /* or IApiKeyService if you have one */) =>
            {
                var userId = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
                if (userId == null) return TypedResults.Unauthorized();

                var tuser = await userManager.FindByIdAsync(userId);
                if (tuser == null) return TypedResults.Unauthorized();

                // generate plaintext key (32 bytes -> base64, user sees it once)
                var keyBytes = RandomNumberGenerator.GetBytes(32);
                var plainKey = Convert.ToBase64String(keyBytes);

                // hash before storage
                var hashed = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(plainKey)));

                // Persist the hashed key + metadata using tokenService or a dedicated IApiKeyService
                // Assume tokenService has CreateApiKeyAsync(userId, name, hashed, scopes)
               // var apiKey = await tokenService.CreateApiKeyAsync(tuser.Id, req.Name, hashed, req.Scopes ?? Array.Empty<string>());

                //var response = new ApiKeyResponse
                //{
                //    Id = apiKey.Id,
                //    Name = apiKey.Name,
                //    Key = plainKey,              // return plaintext only now
                //    CreatedAt = apiKey.CreatedAt
                //};

                //// return 201 with location header (optional)
                //return TypedResults.Created($"/api/apikeys/{apiKey.Id}", response);

                return TypedResults.BadRequest("Not implemented: API key storage logic missing");
            })
            .WithSummary("Create API key")
            .WithDescription("Creates a one-time-view API key for the authenticated user. The plaintext key is returned only once at creation.");

        return routes;
    }
}
