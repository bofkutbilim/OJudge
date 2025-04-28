using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Dtos;
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
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<Problem>>> GetAllProblems()
        {
            return await _context.Problems.ToListAsync();
        }

        /// <summary>
        /// Вывод задачи по id
        /// </summary>
        [HttpGet("get{id}")]
        public async Task<ActionResult<Problem>> GetProblemById(int id)
        {
            var problem = await _context.Problems.FindAsync(id);

            if (problem == null)
            {
                return NotFound();
            }

            return problem;
        }

        /// <summary>
        /// добавление задачи
        /// </summary>
        [HttpPost("post")]
        public async Task<ActionResult<Problem>> CreateProblem(ProblemWithoutId problemWithoutId)
        {
            if (problemWithoutId is null)
                return BadRequest();

            var problem = new Problem
            {
                Title = problemWithoutId.Title,
                TimeLimitSec = problemWithoutId.TimeLimitSec,
                MemoryLimitMb = problemWithoutId.MemoryLimitMb
            };

            _context.Problems.Add(problem);
            await _context.SaveChangesAsync();

            return problem;
        }

        /// <summary>
        /// удалить задачу по id
        /// </summary>
        [HttpDelete("delete{id}")]
        public async Task<ActionResult<Problem>> DeleteProblem(int id)
        {
            var problem = await _context.Problems.FindAsync(id);
            if (problem is null)
            {
                return NotFound();
            }

            _context.Problems.Remove(problem);
            await _context.SaveChangesAsync();

            return problem;
        }

        /// <summary>
        /// Изменение задачи
        /// </summary>
        [HttpPut("update{id}")]
        public async Task<ActionResult<Problem>> UpdateProblem(int id, ProblemWithoutId problemWithoutId)
        {

            var problem = await _context.Problems.FindAsync(id);

            if (problem is null)
                return NotFound();

            problem.Title = problemWithoutId.Title;

            if (problemWithoutId.TimeLimitSec is not null)
                problem.TimeLimitSec = problemWithoutId.TimeLimitSec;
            if (problemWithoutId.TimeLimitSec is not null)
                problem.MemoryLimitMb = problemWithoutId.MemoryLimitMb;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProblemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return problem;
        }

        private bool ProblemExists(int id)
        {
            return _context.Problems.Any(p => p.Id == id);
        }
    }
}
