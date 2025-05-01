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

        public async Task<IEnumerable<Topic>> GetAllAsync() {
            return await _context.Topics.ToListAsync();
        }

        public async Task<Topic?> GetByIdAsync(int id) {
            var get = await _context.Topics.FindAsync(id);
            return get;
        }
        public async Task<Topic?> CreateAsync(TopicShortDto dto) {
            if (dto == null) return null;

            var created = new Topic
            {
                Title = dto.Title,
                Description = dto.Description
            };

            await _context.Topics.AddAsync(created);
            await _context.SaveChangesAsync();
            return created;
        }
        public async Task<Topic?> UpdateAsync(int id, TopicShortDto dto) {
            if (dto is null) return null;
            
            var updated = await _context.Topics.FindAsync(id);
            if (updated is null) return null;
        
            updated.Title = dto.Title;
            if (dto.Description is not null)
                updated.Description = dto.Description;

            await _context.SaveChangesAsync();
            return updated;
        }
        public async Task<Topic?> DeleteAsync(int id) {
            var topic = await _context.Topics.FindAsync(id);
            if (topic is null) return null;

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return topic;
        }
    }
}
