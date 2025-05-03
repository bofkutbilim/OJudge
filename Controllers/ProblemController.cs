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
        public async Task<ActionResult<IEnumerable<ProblemShortDto>>> GetAllProblems()
        {
            return Ok(await _problemService.GetAllAsync());
        }

        /// <summary>
        /// Вывод задачи по id
        /// </summary>
        [HttpGet("get{id}")]
        public async Task<ActionResult<ProblemDto>> GetProblemById(int id)
        {
            var problem = await _problemService.GetByIdAsync(id);
            return problem is null ? NotFound() : Ok(problem);
        }

        /// <summary>
        /// добавление задачи
        /// </summary>
        [HttpPost("post")]
        public async Task<ActionResult<ProblemShortDto>> CreateProblem(CreateProblemDto dto)
        {
            if (dto is null) return BadRequest();
            var created = await _problemService.CreateAsync(dto);
            return Ok(created);
        }

        /// <summary>
        /// удалить задачу по id
        /// </summary>
        [HttpDelete("delete{id}")]
        public async Task<ActionResult<ProblemShortDto>> DeleteProblem(int id)
        {
            var deleted = await _problemService.DeleteAsync(id);
            return deleted == null ? NotFound() : Ok(deleted);
        }

        /// <summary>
        /// Изменение задачи
        /// </summary>
        [HttpPut("update{id}")]
        public async Task<ActionResult<ProblemShortDto>> UpdateProblem(int id, UpdateProblemDto dto)
        {
            var updated = await _problemService.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }


        /// <summary>
        /// Добавление темы в задачу
        /// </summary>
        [HttpPut("addtopic{id}/{topicId}")]
        public async Task<ActionResult<ProblemDto>> AddTopicToProblem(int id, int topicId)
        {
            var updated = await _problemService.AddTopicAsync(id, topicId);
            return updated is null ? NotFound() : Ok(updated);
        }


        [HttpPut("addinfo{id}")]
        public async Task<ActionResult<ProblemDto>> AddProblemInformationToProblem(int id, CreateProblemInformationDto dto)
        {
            var updated = await _problemService.AddSectionAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("deletetopic{id}/{topicId}")]
        public async Task<ActionResult<ProblemDto>> DeleteTopicFromProblem(int id, int topicId)
        {
            var changed = await _problemService.DeleteTopicAsync(id, topicId);
            return changed is null ? NotFound() : Ok(changed);
        }

        [HttpDelete("deletesection{id}/{sectionId}")]
        public async Task<ActionResult<ProblemDto>> DeleteProblemInformationFromProblem(int id, int sectionId)
        {
            var changed = await _problemService.DeleteSectionAsync(id, sectionId);
            return changed is null ? NotFound() : Ok(changed);
        }
    }
}
