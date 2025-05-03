using OJudge.Models;
using OJudge.Dtos;

namespace OJudge.Services
{
    public interface ITopicService
    {
        Task<IEnumerable<TopicShortDto>> GetAllAsync();
        //Task<Topic?> GetByIdAsync(int id);
        Task<TopicShortDto?> CreateAsync(CreateTopicDto dto);
        Task<TopicShortDto?> UpdateAsync(int id, UpdateTopicDto dto);
        Task<TopicShortDto?> DeleteAsync(int id);
    }
}
