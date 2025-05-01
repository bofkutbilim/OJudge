using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Dtos;
using OJudge.Models;
using OJudge.Services;

namespace OJudge.Controllers
{
    [ApiController]
    [Route("api/problem")]
    public class ProblemController : ControllerBase
    {
        private readonly IProblemService _problemService;

        public ProblemController(IProblemService problemService)
        {
            _problemService = problemService;
        }

        /// <summary>
        /// Вывод всех задач
        /// </summary>
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<Problem>>> GetAllProblems()
        {
            return Ok(await _problemService.GetAllAsync());
        }

        /// <summary>
        /// Вывод задачи по id
        /// </summary>
        [HttpGet("get{id}")]
        public async Task<ActionResult<Problem>> GetProblemById(int id)
        {
            var problem = await _problemService.GetByIdAsync(id);
            return problem == null ? NotFound() : Ok(problem);
        }

        /// <summary>
        /// добавление задачи
        /// </summary>
        [HttpPost("post")]
        public async Task<ActionResult<Problem>> CreateProblem(ProblemWithoutId problemWithoutId)
        {
            if (problemWithoutId == null) return BadRequest();
            var created = await _problemService.CreateAsync(problemWithoutId);
            return Ok(created);
        }

        /// <summary>
        /// удалить задачу по id
        /// </summary>
        [HttpDelete("delete{id}")]
        public async Task<ActionResult<Problem>> DeleteProblem(int id)
        {
            var deleted = await _problemService.DeleteAsync(id);
            return deleted == null ? NotFound() : Ok(deleted);
        }

        /// <summary>
        /// Изменение задачи
        /// </summary>
        [HttpPut("update{id}")]
        public async Task<ActionResult<Problem>> UpdateProblem(int id, ProblemWithoutId problemWithoutId)
        {
            var updated = await _problemService.UpdateAsync(id, problemWithoutId);
            return updated == null ? NotFound() : Ok(updated);
        }


        /// <summary>
        /// Добавление раздела в задачу
        /// </summary>
        [HttpPut("addsection{id}")]
        public async Task<ActionResult<Problem>> AddSectionToProblem(int id, ProblemPageShortDto dto)
        {
            var updated = await _problemService.AddSectionAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}
