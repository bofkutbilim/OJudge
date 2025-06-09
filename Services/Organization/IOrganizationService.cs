using OJudge.Models;

namespace OJudge.Services
{
    public interface IOrganizationService
    {
        Task<IEnumerable<Organization>> GetAllAsync();
    }
}
