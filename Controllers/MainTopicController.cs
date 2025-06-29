using Microsoft.AspNetCore.Mvc;
using OJudge.Dtos;
using OJudge.Models;
using OJudge.Services;

namespace OJudge.Controllers
{
    [ApiController]
    [Route("api/maintopic")]
    public class MainTopicController : ControllerBase
    {
        private readonly IMainTopicService _mainTopicService;
        public MainTopicController(IMainTopicService mainTopicService)
        {
            _mainTopicService = mainTopicService;
        }

        /// <summary>
        /// Вывод всех задач
        /// </summary>
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<MainTopicDto>>> GetAllMainTopics()
        {
            return Ok(await _mainTopicService.GetAllAsync());
        }

        [HttpGet("get{id}")]
        public async Task<ActionResult<MainTopic?>> GetMainTopicById(int id)
        {
            var mt = await _mainTopicService.GetByIdAsync(id);
            return mt is null ? NotFound() : Ok(mt);
        }

        [HttpPost("create")]
        public async Task<ActionResult<MainTopic>> CreateMainTopic(CreateMainTopicDto dto)
        {
            if (dto is null) return BadRequest();
            var created = await _mainTopicService.CreateAsync(dto);
            return created is null ? NotFound() : Ok(created);
        }

        [HttpPut("update{id}")]
        public async Task<ActionResult<MainTopic?>> UpdateMainTopic(int id, CreateMainTopicDto dto)
        {
            if (dto is null) return BadRequest();
            var updated = await _mainTopicService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("delete{id}")]
        public async Task<ActionResult<MainTopic?>> DeleteMainTopic(int id)
        {
            var deleted = await _mainTopicService.DeleteAsync(id);
            return deleted is null ? NotFound() : Ok(deleted);
        }
    }
}
