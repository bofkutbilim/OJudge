using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJudge.Dtos;
using OJudge.Models;
using OJudge.Services;

namespace OJudge.Controllers;

[ApiController]
[Route("api/user")]
public class UsersController(IUserService userService, JwtSessionService sessions) : ControllerBase
{
    [HttpGet("get")]
    public async Task<ActionResult<IEnumerable<UserRanklistDto>>> GetAllUsers()
    {
        return Ok(await userService.GetAllAsync());
    }

    [HttpGet("gettop")]
    public async Task<ActionResult<IEnumerable<UserRanklistDto>>> GetTopUsers()
    {
        return Ok(await userService.GetAllAsync());
    }

    [HttpGet("get{id}")]
    public async Task<ActionResult<UserProfileDto>> GetUserById(int id)
    {
        var user = await userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("create")]
    public async Task<ActionResult<UserProfileDto>> CreateUser(CreateUserDto dto, CancellationToken ct)
    {
        var profile = await userService.CreateAsync(dto, ct);
        if (profile is null) return Conflict(new { message = "Этот никнейм уже занят." });
        return CreatedAtAction(nameof(GetUserById), new { id = profile.Id }, profile);
    }

    [Authorize]
    [HttpPut("update{id}")]
    public async Task<ActionResult<UserProfileDto>> UpdateUser(int id, UpdateUserDto dto, CancellationToken ct)
    {
        if (User.GetUserId() != id && !User.IsInRole(AuthRoles.Admin)) return Forbid();

        var profile = await userService.UpdateAsync(id, dto, ct);
        return profile is null ? NotFound() : Ok(profile);
    }

    [Authorize(Roles = AuthRoles.Admin)]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser([FromQuery] int id, CancellationToken ct)
    {
        return await userService.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }

    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [HttpPut("login")]
    public async Task<ActionResult<TokenDto>> Login(LoginDataDto dto, CancellationToken ct)
    {
        var user = await userService.AuthenticateAsync(dto, ct);
        if (user is null) return Unauthorized(new { message = "Неверный никнейм или пароль." });
        return Ok(await sessions.IssueAsync(user, ct));
    }

    [Authorize]
    [HttpPut("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await sessions.RevokeAsync(User, ct);
        return NoContent();
    }

    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [HttpGet("isactive")]
    public ActionResult<UserActiveDto> IsActive()
    {
        if (User.GetUserId() is not int id || User.Identity?.Name is not string nickname ||
            User.FindFirst("role")?.Value is not string role)
            return Unauthorized();

        return Ok(new UserActiveDto { UserId = id, UserNickName = nickname, UserRole = role });
    }
}
