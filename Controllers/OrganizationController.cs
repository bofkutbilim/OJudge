using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJudge.Dtos;
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

        [Authorize(Roles = AuthRoles.Admin)]
        [HttpPost("create")]
        public async Task<ActionResult<Organization>> CreateOrganization(CreateOrganizationDto dto, CancellationToken ct)
        {
            var organization = await _organizationService.CreateAsync(dto, ct);
            return StatusCode(StatusCodes.Status201Created, organization);
        }
    }
}
