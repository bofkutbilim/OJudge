using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJudge.Dtos;
using OJudge.Models;
using OJudge.Services;

namespace OJudge.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;
        public TestController(ITestService testService) {
            _testService = testService;
        }

        [HttpDelete("delete{id}")]
        public async Task<ActionResult<Test?>> DeleteTest(int id) {
            var deleted = await _testService.DeleteAsync(id);
            return deleted is null ? NotFound() : Ok(deleted);
        }

        [HttpPut("update{id}")]
        public async Task<ActionResult<Test?>> UpdateTest(int id, UpdateTestDto dto) {
            if (dto is null) return BadRequest();
            var updated = await _testService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }
    }
}
