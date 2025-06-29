using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJudge.Models;
using OJudge.Services;

namespace OJudge.Controllers
{
    [Route("api/organization")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<Organization>>> GetAllCountries()
        {
            return Ok(await _organizationService.GetAllAsync());
        }
    }
}
