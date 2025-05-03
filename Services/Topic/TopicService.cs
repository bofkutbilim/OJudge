using OJudge.Data;
using OJudge.Models;
using OJudge.Dtos;
using Microsoft.EntityFrameworkCore;

namespace OJudge.Services
{
    public class TopicService : ITopicService
    {
        private readonly AppDbContext _context;

        public TopicService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TopicShortDto>> GetAllAsync() {
            var ret = new List<TopicShortDto>();
            foreach (var x in await _context.Topics.ToListAsync())
                ret.Add(new TopicShortDto { Id = x.Id, Title = x.Title, Description = x.Description });
            return ret;
        }

        //public async Task<Topic?> GetByIdAsync(int id) {
        //    var get = await _context.Topics.FindAsync(id);
        //    return get;
        //}
        public async Task<TopicShortDto?> CreateAsync(CreateTopicDto dto) {
            if (dto == null) return null;

            var created = new Topic
            {
                Title = dto.Title,
                Description = dto.Description
            };

            await _context.Topics.AddAsync(created);
            await _context.SaveChangesAsync();

            return new TopicShortDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description
            };
        }
        public async Task<TopicShortDto?> UpdateAsync(int id, UpdateTopicDto dto)
        {
            if (dto is null) return null;

            var updated = await _context.Topics.FindAsync(id);
            if (updated is null) return null;

            if (dto.Title is not null)
                updated.Title = dto.Title;
            if (dto.Description is not null)
                updated.Description = dto.Description;

            await _context.SaveChangesAsync();
            return new TopicShortDto
            {
                Id = updated.Id,
                Title = updated.Title,
                Description = updated.Description
            };
        }
        public async Task<TopicShortDto?> DeleteAsync(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic is null) return null;

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return new TopicShortDto
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description
            };
        }
    }
}
