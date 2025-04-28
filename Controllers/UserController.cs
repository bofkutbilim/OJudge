using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Models;
using OJudge.Dtos;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace OJudge.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Вывод всех пользователей
        /// </summary>
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Вывод пользователя по id
        /// </summary>
        [HttpGet("get{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// <summary>
        /// добавление пользователя
        /// </summary>
        [HttpPost("post")]
        public async Task<ActionResult<User>> CreateUser(UserWithoutId userWithoutId)
        {
            if (userWithoutId is null)
                return BadRequest();

            if (await _context.Users.FirstOrDefaultAsync(u => u.NickName == userWithoutId.NickName) is not null)
                return BadRequest(new { message = "Такой пользователь уже существует." });

            var user = new User { NickName = userWithoutId.NickName };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Изменение пользователя
        /// </summary>
        [HttpPut("update{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, UserWithoutId userWithoutId)
        {

            var user = await _context.Users.FindAsync(id);

            if (user is null)
                return NotFound();

            user.NickName = userWithoutId.NickName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return user;
        }

        /// <summary>
        /// удаление пользователя
        /// </summary>
        [HttpDelete("delete{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
