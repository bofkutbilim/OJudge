using Microsoft.AspNetCore.Authorization;
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
            return Ok(await _problemService.GetAllAsync(User.GetUserId()));
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

        [HttpGet("get/filter/{minDif}/{maxDif}/{solved}")]
        public async Task<ActionResult<IEnumerable<ProblemShortDto>>> GetProblemsByFilter(int minDif, int maxDif, string solved, [FromQuery] string? str)
        {
            return Ok(await _problemService.GetAllByFilterAsync(str, minDif, maxDif, solved, User.GetUserId()));
        }

        [HttpGet("get/bytopic{topicId}")]
        public async Task<ActionResult<IEnumerable<ProblemShortDto>>> GetProblemByTopicId(int topicId)
        {
            return Ok(await _problemService.GetAllByTopicAsync(topicId, User.GetUserId()));
        }

        [Authorize(Roles = AuthRoles.Admin)]
        [HttpGet("get/tests{id}")]
        public async Task<ActionResult<IEnumerable<Test>>> GetProblemTests(int id)
        {
            return Ok(await _problemService.GetTestsAsync(id));
        }

        [Authorize(Roles = AuthRoles.Admin)]
        [HttpPost("create")]
        public async Task<ActionResult<ProblemShortDto>> CreateProblem(CreateProblemDto dto)
        {
            if (dto is null) return BadRequest(new { message = "Введите данные" });
            var created = await _problemService.CreateAsync(dto);
            return Ok(new { problem = created, message = "Добавлен успешно!"});
        }

        /// <summary>
        /// удалить задачу по id
        /// </summary>
        [Authorize(Roles = AuthRoles.Admin)]
        [HttpDelete("delete{id}")]
        public async Task<ActionResult<ProblemShortDto>> DeleteProblem(int id)
        {
            var deleted = await _problemService.DeleteAsync(id);
            return deleted is null ? NotFound() : Ok(new {problem = deleted, message = "Удалён успешно!"});
        }

        /// <summary>
        /// Изменение задачи
        /// </summary>
        [Authorize(Roles = AuthRoles.Admin)]
        [HttpPut("update{id}")]
        public async Task<ActionResult<ProblemShortDto>> UpdateProblem(int id, UpdateProblemDto dto)
        {
            if (dto is null) return BadRequest(new { message = "Введите данные" });
            
            var updated = await _problemService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(new { problem = updated, message = "Изменён успешно!" });
        }

        [Authorize(Roles = AuthRoles.Admin)]
        [HttpPut("addtopic{id}/{topicId}")]
        public async Task<ActionResult<ProblemDto?>> AddTopicToProblem(int id, int topicId)
        {
            var updated = await _problemService.AddTopicAsync(id, topicId);
            return updated is null ? NotFound() : Ok(new { problem = updated, message = "Тема добавлена успешно" });
        }

        [Authorize(Roles = AuthRoles.Admin)]
        [HttpDelete("deletetopic{id}/{topicId}")]
        public async Task<ActionResult<ProblemDto?>> DeleteTopicFromProblem(int id, int topicId)
        {
            var deleted = await _problemService.DeleteTopicAsync(id, topicId);
            return deleted is null ? NotFound() : Ok(deleted);
        }

        [HttpGet("gettopics/byproblem{id}")]
        public async Task<ActionResult<IEnumerable<TopicShortDto>>> GetTopicsByProblemId(int id) {
            return Ok(await _problemService.GetTopicsAsync(id));
        }

        [Authorize(Roles = AuthRoles.Admin)]
        [HttpPut("addsection{id}")]
        public async Task<ActionResult<ProblemDto>> AddSectionToProblem(int id, CreateProblemInformationDto dto) {
            if (dto is null) return BadRequest(new { message = "Введите данные" });
            var updated = await _problemService.AddSectionAsync(id, dto);
            return updated is null ? NotFound() : Ok(new { problem = updated, message = "Секция добавлена успешно!" });
        }


        [Authorize(Roles = AuthRoles.Admin)]
        [HttpPut("addtest{id}")]
        public async Task<ActionResult<Test>> AddTestToProblem(int id, CreateTestDto dto)
        {
            if (dto is null) return BadRequest(new { message = "Введите данные" });
            var created = await _problemService.AddTestAsync(id, dto);
            return created is null ? NotFound() : Ok(new { test = created, message = "Тест добавлен успешно" });
        }
    }
}
