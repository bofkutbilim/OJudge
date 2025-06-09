using OJudge.Models;

namespace OJudge.Services
{
    public interface ICountryService
    {
        Task<IEnumerable<Country>> GetAllAsync();
    }
}
