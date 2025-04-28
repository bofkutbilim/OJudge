using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Models;

namespace OJudge.Controllers
{
    [ApiController]
    [Route("api/problem")]
    public class ProblemController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProblemController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Вывод всех задач
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Problem>>> GetAllProblems()
        {
            return await _context.Problems.ToListAsync();
        }

        /// <summary>
        /// Вывод задачи по id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Problem>> GetProblemById(int id)
        {
            var problem = await _context.Problems.FindAsync(id);

            if (problem == null)
            {
                return NotFound();
            }

            return problem;
        }
    }
}
