using AuthAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Features.Users;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("register"), AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest dto)
    {
        var result = await _userService.RegisterAsync(dto);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    [HttpGet, AdminAuthorize]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }

    [HttpPost("{id}/roles"), AdminAuthorize]
    public async Task<IActionResult> AddRole(Guid id, RoleRequest dto)
    {
        var result = await _userService.AddRoleToUserAsync(id, dto.RoleName);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    [HttpPost("{id}/reset-password"), AdminAuthorize]
    public async Task<IActionResult> ResetPassword(Guid id, ResetPasswordRequest dto)
    {
        var result = await _userService.ResetPasswordAsync(id, dto.NewPassword);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    [HttpDelete("{id}"), AdminAuthorize]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }
}
