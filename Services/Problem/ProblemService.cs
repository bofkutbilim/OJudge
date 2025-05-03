using OJudge.Data;
using OJudge.Dtos;
using OJudge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace OJudge.Services
{
    public class ProblemService : IProblemService
    {
        private readonly AppDbContext _context;

        public ProblemService(AppDbContext context)
        {
            _context = context;
        }

        public ProblemDto Problem_To_ProblemDto(Problem problem)
        {
            var ret = new ProblemDto
            {
                Id = problem.Id,
                Title = problem.Title,
                TimeLimitSec = problem.TimeLimitSec,
                MemoryLimitMB = problem.MemoryLimitMB,
                Topics = new List<TopicShortDto>(),
                ProblemInformations = new List<ProblemInformationShortDto>()
            };

            foreach (var x in problem.Topics)
            {
                ret.Topics.Add(new TopicShortDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description
                });
            }

            foreach (var x in problem.ProblemInformations)
            {
                ret.ProblemInformations.Add(new ProblemInformationShortDto
                {
                    Id = x.Id,
                    SectionName = x.SectionName,
                    SectionText = x.SectionText
                });
            }

            return ret;
        }
        public async Task<IEnumerable<ProblemShortDto>> GetAllAsync()
        {
            var ret = new List<ProblemShortDto>();
            
            foreach (var x in await _context.Problems.ToListAsync())
                ret.Add(new ProblemShortDto
                {
                    Id = x.Id,
                    Title = x.Title
                });

            return ret;
        }

        public async Task<ProblemDto?> GetByIdAsync(int id)
        {
            var problem = await _context.Problems.Include(p => p.Topics).Include(p => p.ProblemInformations).FirstOrDefaultAsync(p => p.Id == id);
            if (problem is null)
                return null;
            return Problem_To_ProblemDto(problem);
        }

        public async Task<ProblemShortDto?> CreateAsync(CreateProblemDto dto)
        {
            if (dto is null) return null;

            var problem = new Problem
            {
                Title = dto.Title,
                TimeLimitSec = dto.TimeLimitSec,
                MemoryLimitMB = dto.MemoryLimitMB
            };

            await _context.Problems.AddAsync(problem);
            await _context.SaveChangesAsync();

            return new ProblemShortDto
            {
                Id = problem.Id,
                Title = problem.Title
            };
        }

        public async Task<ProblemShortDto?> UpdateAsync(int id, UpdateProblemDto dto)
        {
            if (dto is null) return null;

            var problem = await _context.Problems.FindAsync(id);
            if (problem is null) return null;

            if (dto.Title is not null)
                problem.Title = dto.Title;

            if (dto.TimeLimitSec is not null)
                problem.TimeLimitSec = dto.TimeLimitSec;

            if (dto.MemoryLimitMB is not null)
                problem.MemoryLimitMB = dto.MemoryLimitMB;

            await _context.SaveChangesAsync();

            return new ProblemShortDto
            {
                Id = problem.Id,
                Title = problem.Title
            };
        }

        public async Task<ProblemShortDto?> DeleteAsync(int id)
        {
            var problem = await _context.Problems.FindAsync(id);
            if (problem is null) return null;

            _context.Problems.Remove(problem);
            await _context.SaveChangesAsync();

            return new ProblemShortDto
            {
                Id = problem.Id,
                Title = problem.Title
            };
        }

        public async Task<ProblemDto?> AddTopicAsync(int id, int topicId)
        {
            var top = await _context.Topics.FindAsync(topicId);
            if (top is null)
                return null;

            var problem = await _context.Problems
                .Include(p => p.Topics)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (problem is null)
                return null;

            if (!problem.Topics.Contains(top))
                problem.Topics.Add(top);


            await _context.SaveChangesAsync();

            return Problem_To_ProblemDto(problem);
        }

        public async Task<ProblemDto?> AddSectionAsync(int id, CreateProblemInformationDto dto)
        {
            if (dto is null) return null;

            var problemInfo = new ProblemInformation
            {
                SectionName = dto.SectionName,
                SectionText = dto.SectionText
            };

            var problem = await _context.Problems
                .Include(p => p.ProblemInformations)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (problem is null) return null;

            problem.ProblemInformations.Add(problemInfo);
            await _context.SaveChangesAsync();

            return Problem_To_ProblemDto(problem);
        }

        public async Task<ProblemDto?> DeleteTopicAsync(int id, int topicId)
        {
            var top = await _context.Topics.FindAsync(topicId);
            if (top is null) return null;

            var problem = await _context.Problems
                .Include(p => p.Topics)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (problem is null) return null;

            problem.Topics.Remove(top);
            await _context.SaveChangesAsync();

            return Problem_To_ProblemDto(problem);
        }
        public async Task<ProblemDto?> DeleteSectionAsync(int id, int sectionId)
        {
            var sec = await _context.ProblemInformations.FindAsync(sectionId);
            if (sec is null) return null;

            var problem = await _context.Problems
                .Include(p => p.ProblemInformations)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (problem is null) return null;

            problem.ProblemInformations.Remove(sec);
            await _context.SaveChangesAsync();

            return Problem_To_ProblemDto(problem);
        }
    }
}
