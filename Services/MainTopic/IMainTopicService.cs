using OJudge.Dtos;
using OJudge.Models;

namespace OJudge.Services
{
    public interface IMainTopicService
    {
        Task<IEnumerable<MainTopicDto>> GetAllAsync();
        Task<MainTopic?> GetByIdAsync(int id);
        Task<MainTopic> CreateAsync(CreateMainTopicDto dto);
        Task<MainTopic?> DeleteAsync(int id);
        Task<MainTopic?> UpdateAsync(int id, CreateMainTopicDto dto);
    }
}
