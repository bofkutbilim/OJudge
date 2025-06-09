using OJudge.Dtos;
using OJudge.Data;
using Microsoft.EntityFrameworkCore;
using OJudge.Models;

namespace OJudge.Services
{
    public class MainTopicService : IMainTopicService
    {
        private readonly AppDbContext _context;
        public MainTopicService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MainTopicDto>> GetAllAsync()
        {
            var ret = new List<MainTopicDto>();

            foreach (var x in await _context.MainTopics.Include(mt => mt.Topics).ThenInclude(t => t.Problems).ToListAsync())
            {
                var nx = new MainTopicDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Topics = new List<TopicListDto>()
                };

                foreach (var topic in x.Topics) {
                    var topicListDto = new TopicListDto
                    {
                        Id = topic.Id,
                        Title = topic.Title,
                        Undefined = 0,
                        Easy = 0,
                        Medium = 0,
                        Hard = 0
                    };

                    foreach (var problem in topic.Problems)
                    {
                        if (problem.Difficulty is null) topicListDto.Undefined++;
                        else if (problem.Difficulty < 30) topicListDto.Easy++;
                        else if (problem.Difficulty < 60) topicListDto.Medium++;
                        else topicListDto.Hard++;
                    }

                    nx.Topics.Add(topicListDto);
                }

                ret.Add(nx);
            }
            return ret;
        }

        public async Task<MainTopic> CreateAsync(CreateMainTopicDto dto)
        {
            MainTopic mTopic = new MainTopic
            {
                Title = dto.Title
            };

            await _context.MainTopics.AddAsync(mTopic);
            await _context.SaveChangesAsync();

            return mTopic;
        }

        public async Task<MainTopic?> DeleteAsync(int id)
        {
            MainTopic? mTopic = await _context.MainTopics.FindAsync(id);
            if (mTopic is null)
                return null;

            _context.MainTopics.Remove(mTopic);
            await _context.SaveChangesAsync();

            return mTopic;

        }

        public async Task<MainTopic?> UpdateAsync(int id, CreateMainTopicDto dto)
        {
            MainTopic? mTopic = await _context.MainTopics.FindAsync(id);
            if (mTopic is null)
                return null;

            mTopic.Title = dto.Title;
            await _context.SaveChangesAsync();

            return mTopic;
        }

        public async Task<MainTopic?> GetByIdAsync(int id)
        {
            var mt = await _context.MainTopics.FindAsync(id);
            if (mt is null) return null;
            return mt;
        }
    }
}
