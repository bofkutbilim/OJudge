using OJudge.Dtos;
using OJudge.Models;

namespace OJudge.Services
{
    public interface IProblemService
    {
        Task<IEnumerable<Problem>> GetAllAsync();
        Task<Problem?> GetByIdAsync(int id);
        Task<Problem?> CreateAsync(ProblemWithoutId dto);
        Task<Problem?> UpdateAsync(int id, ProblemWithoutId dto);
        Task<Problem?> DeleteAsync(int id);
        Task<Problem?> AddSectionAsync(int id, ProblemPageShortDto dto);
    }
}
