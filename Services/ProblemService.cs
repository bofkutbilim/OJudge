using OJudge.Data;
using OJudge.Dtos;
using OJudge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OJudge.Services
{
    public class ProblemService : IProblemService
    {
        private readonly AppDbContext _context;

        public ProblemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Problem>> GetAllAsync()
        {
            return await _context.Problems.Include(p => p.ProblemPages).ToListAsync();
        }

        public async Task<Problem?> GetByIdAsync(int id)
        {
            return await _context
                .Problems
                .Include(p => p.ProblemPages)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Problem> CreateAsync(ProblemWithoutId dto)
        {
            if (dto is null) return null;

            var problem = new Problem
            {
                Title = dto.Title,
                TimeLimitSec = dto.TimeLimitSec,
                MemoryLimitMb = dto.MemoryLimitMb
            };

            await _context.Problems.AddAsync(problem);
            await _context.SaveChangesAsync();

            return problem;
        }

        public async Task<Problem?> UpdateAsync(int id, ProblemWithoutId dto)
        {
            if (dto is null) return null;

            var problem = await _context.Problems.Include(p => p.ProblemPages).FirstOrDefaultAsync(p => p.Id == id);
            if (problem is null) return null;

            problem.Title = dto.Title;

            if (dto.TimeLimitSec is not null)
                problem.TimeLimitSec = dto.TimeLimitSec;

            if (dto.MemoryLimitMb is not null)
                problem.MemoryLimitMb = dto.MemoryLimitMb;

            await _context.SaveChangesAsync();
            return problem;
        }

        public async Task<Problem?> DeleteAsync(int id)
        {
            var problem = await _context.Problems.Include(p => p.ProblemPages).FirstOrDefaultAsync(p => p.Id == id);
            if (problem is null) return null;

            _context.Problems.Remove(problem);
            await _context.SaveChangesAsync();

            return problem;
        }

        public async Task<Problem?> AddSectionAsync(int id, ProblemPageShortDto dto)
        {
            if (dto is null) return null;

            var problem = await _context
                .Problems
                .Include(p => p.ProblemPages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (problem is null) return null;

            var problemPage = new ProblemPage
            {
                TextHtml = dto.TextHtml,
                Section = dto.Section,
                ProblemId = problem.Id
            };
            problem.ProblemPages.Add(problemPage);
            await _context.SaveChangesAsync();

            return problem;
        }
    }
}
