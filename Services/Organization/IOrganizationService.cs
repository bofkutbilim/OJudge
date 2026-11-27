using OJudge.Models;

namespace OJudge.Services
{
    public interface IOrganizationService
    {
        Task<IEnumerable<Organization>> GetAllAsync();
        Task<Organization> CreateAsync(OJudge.Dtos.CreateOrganizationDto dto, CancellationToken ct);
    }
}
