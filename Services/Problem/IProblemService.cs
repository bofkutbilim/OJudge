using OJudge.Dtos;
using OJudge.Models;
using Microsoft.AspNetCore.Mvc;

namespace OJudge.Services
{
    public interface IProblemService
    {
        Task<IEnumerable<ProblemShortDto>> GetAllAsync();
        Task<ProblemDto?> GetByIdAsync(int id);
        Task<ProblemShortDto?> CreateAsync(CreateProblemDto dto);
        Task<ProblemShortDto?> UpdateAsync(int id, UpdateProblemDto dto);
        Task<ProblemShortDto?> DeleteAsync(int id);
        Task<ProblemDto?> AddTopicAsync(int id, int topicId);
        Task<ProblemDto?> AddSectionAsync(int id, CreateProblemInformationDto dto);
        Task<ProblemDto?> DeleteTopicAsync(int id, int topicId);
        Task<ProblemDto?> DeleteSectionAsync(int id, int sectionId);
    }
}
