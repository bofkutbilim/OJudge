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
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            return Ok(await _problemService.GetAllAsync(ip));
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
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            return Ok(await _problemService.GetAllByFilterAsync(str, minDif, maxDif, solved, ip));
        }

        [HttpGet("get/bytopic{topicId}")]
        public async Task<ActionResult<IEnumerable<ProblemShortDto>>> GetProblemByTopicId(int topicId)
        {
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            return Ok(await _problemService.GetAllByTopicAsync(topicId, ip));
        }

        [HttpGet("get/tests{id}")]
        public async Task<ActionResult<IEnumerable<Test>>> GetProblemTests(int id)
        {
            return Ok(await _problemService.GetTestsAsync(id));
        }

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
        [HttpDelete("delete{id}")]
        public async Task<ActionResult<ProblemShortDto>> DeleteProblem(int id)
        {
            var deleted = await _problemService.DeleteAsync(id);
            return deleted is null ? NotFound() : Ok(new {problem = deleted, message = "Удалён успешно!"});
        }

        /// <summary>
        /// Изменение задачи
        /// </summary>
        [HttpPut("update{id}")]
        public async Task<ActionResult<ProblemShortDto>> UpdateProblem(int id, UpdateProblemDto dto)
        {
            if (dto is null) return BadRequest(new { message = "Введите данные" });
            
            var updated = await _problemService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(new { problem = updated, message = "Изменён успешно!" });
        }

        [HttpPut("addtopic{id}/{topicId}")]
        public async Task<ActionResult<ProblemDto?>> AddTopicToProblem(int id, int topicId)
        {
            var updated = await _problemService.AddTopicAsync(id, topicId);
            return updated is null ? NotFound() : Ok(new { problem = updated, message = "Тема добавлена успешно" });
        }

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

        [HttpPut("addsection{id}")]
        public async Task<ActionResult<ProblemDto>> AddSectionToProblem(int id, CreateProblemInformationDto dto) {
            if (dto is null) return BadRequest(new { message = "Введите данные" });
            var updated = await _problemService.AddSectionAsync(id, dto);
            return updated is null ? NotFound() : Ok(new { problem = updated, message = "Секция добавлена успешно!" });
        }


        [HttpPut("addtest{id}")]
        public async Task<ActionResult<Test>> AddTestToProblem(int id, CreateTestDto dto)
        {
            if (dto is null) return BadRequest(new { message = "Введите данные" });
            var created = await _problemService.AddTestAsync(id, dto);
            return created is null ? NotFound() : Ok(new { test = created, message = "Тест добавлен успешно" });
        }
    }
}
