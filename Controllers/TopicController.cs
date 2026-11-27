using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJudge.Services;
using OJudge.Models;
using OJudge.Dtos;

namespace OJudge.Controllers
{
    [ApiController]
    [Route("api/topic")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<Topic>>> GetAllTopics()
        {
            return Ok(await _topicService.GetAllAsync());
        }

        [HttpGet("get{id}")]
        public async Task<ActionResult<Topic>> GetTopicById(int id)
        {
            var topic = await _topicService.GetByIdAsync(id);
            return topic is null ? NotFound() : Ok(topic);
        }

        [Authorize(Roles = AuthRoles.Admin)]
        [HttpPost("post{mainTopicId}")]
        public async Task<ActionResult<Topic>> CreateTopic(int mainTopicId, CreateTopicDto dto)
        {
            if (dto is null) return BadRequest();
            var created = await _topicService.CreateAsync(mainTopicId, dto);
            return Ok(created);
        }

        [Authorize(Roles = AuthRoles.Admin)]
        [HttpPut("update{id}")]
        public async Task<ActionResult<Topic>> UpdateTopic(int id, UpdateTopicDto dto)
        {
            if (dto is null) return BadRequest();
            var updated = await _topicService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [Authorize(Roles = AuthRoles.Admin)]
        [HttpDelete("delete{id}")]
        public async Task<ActionResult<Topic>> DeleteTopic(int id)
        {
            var deleted = await _topicService.DeleteAsync(id);
            return deleted is null ? NotFound() : Ok(deleted);
        }
    }
}
