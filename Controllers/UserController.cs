using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Models;
using OJudge.Dtos;

namespace OJudge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Вывод пользователя по id
        /// </summary>
        [HttpGet("{id}")]
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
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(UserWithoutId userWithoutId)
        {
            if (userWithoutId is null)
                return BadRequest();

            var user = new User { NickName = userWithoutId.NickName };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Изменение пользователя
        /// </summary>
        [HttpPut("{id}")]
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
