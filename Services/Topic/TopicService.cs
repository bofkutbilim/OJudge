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
                ret.Add(new TopicShortDto {
                    Id = x.Id,
                    Title = x.Title,
                    ShortTitle = x.ShortTitle,
                    Description = x.Description
                });
            return ret;
        }

        public async Task<Topic?> GetByIdAsync(int id) {
            var get = await _context.Topics.FindAsync(id);
            return get;
        }
        public async Task<TopicShortDto?> CreateAsync(int mainTopicId, CreateTopicDto dto) {
            MainTopic? mainTopic = await _context.MainTopics.FindAsync(mainTopicId);

            if (mainTopic is null)
                return null;

            var created = new Topic
            {
                Title = dto.Title,
                ShortTitle = dto.ShortTitle,
                Description = dto.Description,
                MainTopicId = mainTopicId,
                MainTopic = mainTopic,
            };

            await _context.Topics.AddAsync(created);
            await _context.SaveChangesAsync();

            return new TopicShortDto
            {
                Id = created.Id,
                Title = created.Title,
                ShortTitle = created.ShortTitle,
                Description = created.Description
            };
        }
        public async Task<TopicShortDto?> UpdateAsync(int id, UpdateTopicDto dto)
        {
            var updated = await _context.Topics.FindAsync(id);
            if (updated is null) return null;

            if (dto.Title is not null && dto.Title != string.Empty)
                updated.Title = dto.Title;
            if (dto.ShortTitle is not null && dto.ShortTitle != string.Empty)
                updated.ShortTitle = dto.ShortTitle;
            if (dto.Description is not null && dto.Description != string.Empty)
                updated.Description = dto.Description;

            await _context.SaveChangesAsync();
            
            return new TopicShortDto
            {
                Id = updated.Id,
                Title = updated.Title,
                ShortTitle = updated.ShortTitle,
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
                ShortTitle = topic.ShortTitle,
                Description = topic.Description
            };
        }
    }
}
