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

        public async Task<Organization> CreateAsync(OJudge.Dtos.CreateOrganizationDto dto, CancellationToken ct)
        {
            var organization = new Organization { Title = dto.Title.Trim(), Description = dto.Description?.Trim() };
            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync(ct);
            return organization;
        }
    }
}
