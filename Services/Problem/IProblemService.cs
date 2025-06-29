using OJudge.Dtos;
using OJudge.Models;
using Microsoft.AspNetCore.Mvc;

namespace OJudge.Services
{
    public interface IProblemService
    {
        Task<IEnumerable<ProblemShortDto>> GetAllAsync(string? ip);
        Task<ProblemDto?> GetByIdAsync(int id);
        Task<ProblemShortDto?> CreateAsync(CreateProblemDto dto);
        Task<IEnumerable<ProblemShortDto>> GetAllByFilterAsync(string? str, int minDifficulty, int maxDifficulty, string solved, string? ip);
        Task<ProblemShortDto?> UpdateAsync(int id, UpdateProblemDto dto);
        Task<ProblemShortDto?> DeleteAsync(int id);
        Task<ProblemDto?> AddTopicAsync(int id, int topicId);
        Task<ProblemDto?> DeleteTopicAsync(int id, int topicId);
        Task<IEnumerable<TopicShortDto>> GetTopicsAsync(int id);
        Task<ProblemDto?> AddSectionAsync(int id, CreateProblemInformationDto dto);
        //Task<ProblemDto?> AddMaterialAsync(int id, int materialId);
        Task<ProblemDto?> DeleteSectionAsync(int id, int sectionId);

        Task<IEnumerable<ProblemShortDto>> GetAllByTopicAsync(int topicId, string? ip);

        Task<IEnumerable<Test>> GetTestsAsync(int id);

        Task<Test>? AddTestAsync(int id, CreateTestDto dto);
    }
}
