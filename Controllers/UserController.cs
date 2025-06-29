using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Models;
using OJudge.Dtos;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using OJudge.Services;

namespace OJudge.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<UserRanklistDto>>> GetAllUsers()
        {
            return Ok(await _userService.GetAllAsync());
        }

        [HttpGet("gettop")]
        public async Task<ActionResult<IEnumerable<UserRanklistDto>>> GetTopUsers()
        {
            return Ok(await _userService.GetAllAsync());
        }

        [HttpGet("get{id}")]
        public async Task<ActionResult<UserProfileDto>> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost("create")]
        public async Task<ActionResult<User?>> CreateUser(CreateUserDto dto)
        {
            if (dto is null)
                return BadRequest();

            var created = await _userService.CreateAsync(dto);
            return created is null ? BadRequest() : Ok(created);
        }

        [HttpPut("update{id}")]
        public async Task<ActionResult<User?>> UpdateUser(int id, UpdateUserDto dto)
        {
            if (dto is null) return BadRequest();
            var updated = await _userService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<User?>> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            return deleted is null ? NotFound() : Ok(deleted);
        }

        [HttpPut("login")]
        public async Task<ActionResult<User?>> LoginAsync(LoginDataDto dto)
        {
            if (dto is null)
                return BadRequest();
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var user = await _userService.LoginAsync(dto, ip);
            return user is null ? NotFound() : Ok(new { message = "Успешный вход!" });
        }

        [HttpPut("logout")]
        public async Task<ActionResult<User?>> LogoutAsync()
        {
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var user = await _userService.LogoutAsync(ip);
            return user is null ? BadRequest() : Ok(new { message = "Успешный выход!" });
        }

        [HttpGet("isactive")]
        public async Task<ActionResult<UserActiveDto?>> IsActiveAsync() {
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var user = await _userService.CheckActiveAsync(ip);
            return user is null ? BadRequest(new { message = "Not Active" }) : Ok(user);
        }
    }
}
