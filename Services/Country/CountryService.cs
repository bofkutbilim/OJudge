using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Dtos;
using OJudge.Models;

namespace OJudge.Services
{
    public class CountryService : ICountryService
    {
        private readonly AppDbContext _context;
        public CountryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _context.Countries.ToListAsync();
        }
    }
}
