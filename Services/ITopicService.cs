using OJudge.Models;
using OJudge.Dtos;

namespace OJudge.Services
{
    public interface ITopicService
    {
        Task<IEnumerable<Topic>> GetAllAsync();
        Task<Topic?> GetByIdAsync(int id);
        Task<Topic?> CreateAsync(TopicShortDto dto);
        Task<Topic?> UpdateAsync(int id, TopicShortDto dto);
        Task<Topic?> DeleteAsync(int id);
    }
}
