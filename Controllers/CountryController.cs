using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJudge.Models;
using OJudge.Services;

namespace OJudge.Controllers
{
    [Route("api/country")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<Country>>> GetAllCountries()
        {
            return Ok(await _countryService.GetAllAsync());
        }
    }
}
