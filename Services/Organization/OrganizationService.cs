using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Models;

namespace OJudge.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly AppDbContext _context;
        public OrganizationService(AppDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Organization>> GetAllAsync()
        {
            return await _context.Organizations.ToListAsync();
        }
    }
}
