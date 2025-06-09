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
                TimeLimitMilliseconds = problem.TimeLimiMilliseconds,
                MemoryLimitKB = problem.MemoryLimitKB,
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
        public async Task<IEnumerable<ProblemShortDto>> GetAllAsync(string? ip)
        {
            var token = await _context.Tokens
                .FirstOrDefaultAsync(t => t.IpAddress == ip && t.IsActive == true);

            var ret = new List<ProblemShortDto>();

            foreach (var x in await _context.Problems.Include(p => p.Topics).ToListAsync())
            {
                var nx = new ProblemShortDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Solved = x.Solved,
                    Submitted = x.Submitted,
                    Difficulty = x.Difficulty,
                };

                nx.IsSubmitted = nx.IsSolved = false;

                if (token is not null)
                {
                    var sub = await _context.Submissions
                        .FirstOrDefaultAsync(s => s.UserId == token.UserId && s.ProblemId == nx.Id && s.VerdictId == 8);
                    
                    if (sub is not null)
                        nx.IsSubmitted = nx.IsSolved = true;
                    else
                    {
                        sub = await _context.Submissions
                            .FirstOrDefaultAsync(s => s.UserId == token.UserId && s.ProblemId == nx.Id);
                        if (sub is not null)
                            nx.IsSubmitted = true;
                    }
                }

                foreach (var t in x.Topics)
                {
                    nx.TopicShortTitle.Add(t.ShortTitle);
                }

                ret.Add(nx);
            }

            return ret;
        }

        public async Task<IEnumerable<ProblemShortDto>> GetAllByFilterAsync(string? str, int minDifficulty, int maxDifficulty, string solved, string? ip)
        {

            var token = await _context.Tokens
                .FirstOrDefaultAsync(t => t.IpAddress == ip && t.IsActive == true);

            var problems = await _context.Problems
                .Where(p => str == null || str == string.Empty || p.Title.Contains(str))
                .Where(p => (minDifficulty == -1 && p.Difficulty == null) || (p.Difficulty >= minDifficulty && p.Difficulty <= maxDifficulty))
                .Include(p => p.Topics)
                .ToListAsync();

            var ret = new List<ProblemShortDto>();

            foreach (var x in problems) {
                var nx = new ProblemShortDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Solved = x.Solved,
                    Submitted = x.Submitted,
                    Difficulty = x.Difficulty
                };
                nx.IsSubmitted = nx.IsSolved = false;

                if (token is not null)
                {
                    var sub = await _context.Submissions
                        .FirstOrDefaultAsync(s => s.UserId == token.UserId && s.ProblemId == nx.Id && s.VerdictId == 8);

                    if (sub is not null)
                        nx.IsSubmitted = nx.IsSolved = true;
                    else
                    {
                        sub = await _context.Submissions
                            .FirstOrDefaultAsync(s => s.UserId == token.UserId && s.ProblemId == nx.Id);
                        if (sub is not null)
                            nx.IsSubmitted = true;
                    }
                }

                foreach (var t in x.Topics)
                {
                    nx.TopicShortTitle.Add(t.ShortTitle);
                }

                if (token is null)
                {
                    ret.Add(nx);
                } else {

                    var sub = await _context.Submissions
                            .FirstOrDefaultAsync(s => s.VerdictId == 8 && s.UserId == token.UserId && s.ProblemId == nx.Id);
                    if (solved == "true" && sub is not null) ret.Add(nx);
                    if (solved == "false" && sub is null) ret.Add(nx);
                    if (solved != "true" && solved != "false") ret.Add(nx);
                }
            }
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
            var problem = new Problem
            {
                Title = dto.Title,
                TimeLimiMilliseconds = dto.TimeLimitMilliseconds,
                MemoryLimitKB = dto.MemoryLimitKB
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
            var problem = await _context.Problems.FindAsync(id);
            if (problem is null) return null;

            if (dto.Title is not null)
                problem.Title = dto.Title;

            if (dto.TimeLimitMilliseconds is not null)
                problem.TimeLimiMilliseconds = dto.TimeLimitMilliseconds;

            if (dto.MemoryLimitKB is not null)
                problem.MemoryLimitKB = dto.MemoryLimitKB;

            if (dto.Difficulty is not null)
                problem.Difficulty = dto.Difficulty;

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
            if (problem is null)
                return null;

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

        public async Task<ProblemDto?> DeleteTopicAsync(int id, int topicId)
        {
            var top = await _context.Topics.FindAsync(topicId);
            if (top is null)
                return null;

            var problem = await _context.Problems
                .Include(p => p.Topics)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (problem is null)
                return null;

            problem.Topics.Remove(top);
            await _context.SaveChangesAsync();

            return Problem_To_ProblemDto(problem);
        }

        public async Task<IEnumerable<TopicShortDto>> GetTopicsAsync(int id)
        {
            var lst = await _context.Topics
                .Include(t => t.Problems)
                .Where(t => t.Problems.Any(p => p.Id == id))
                .ToListAsync();
            var ret = new List<TopicShortDto>();
            foreach (var topic in lst) {
                ret.Add(new TopicShortDto
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    ShortTitle = topic.ShortTitle,
                    Description = topic.Description
                });
            }
            return ret;
        }

        public async Task<ProblemDto?> AddSectionAsync(int id, CreateProblemInformationDto dto)
        {
            var problem = await _context.Problems
                .Include(p => p.ProblemInformations)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (problem is null) return null;
            
            var problemInfo = new ProblemInformation
            {
                SectionName = dto.SectionName,
                SectionText = dto.SectionText,
                ProblemId = problem.Id,
                Problem = problem,
            };
            
            problem.ProblemInformations.Add(problemInfo);
            await _context.SaveChangesAsync();

            return Problem_To_ProblemDto(problem);
        }

        public async Task<ProblemDto?> DeleteSectionAsync(int id, int sectionId)
        {
            return null;
            //var sec = await _context.ProblemInformations.FindAsync(sectionId);
            //if (sec is null) return null;

            //var problem = await _context.Problems
            //    .Include(p => p.ProblemInformations)
            //    .FirstOrDefaultAsync(p => p.Id == id);
            //if (problem is null) return null;

            //problem.ProblemInformations.Remove(sec);
            //await _context.SaveChangesAsync();

            //return Problem_To_ProblemDto(problem);
        }

        public async Task<IEnumerable<ProblemShortDto>> GetAllByTopicAsync(int topicId, string? ip)
        {

            var token = await _context.Tokens
                .FirstOrDefaultAsync(t => t.IpAddress == ip && t.IsActive == true);

            var ans = new List<ProblemShortDto>();

            foreach (var x in await _context.Problems
                .Include(p => p.Topics)
                .Where(p => p.Topics.Any(t => t.Id == topicId))
                .ToListAsync())
            {
                var nx = new ProblemShortDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Solved = x.Solved,
                    Submitted = x.Submitted,
                    Difficulty = x.Difficulty
                };

                if (token is not null)
                {
                    var sub = await _context.Submissions
                        .FirstOrDefaultAsync(s => s.UserId == token.UserId && s.ProblemId == nx.Id && s.VerdictId == 8);

                    if (sub is not null)
                        nx.IsSubmitted = nx.IsSolved = true;
                    else
                    {
                        sub = await _context.Submissions
                            .FirstOrDefaultAsync(s => s.UserId == token.UserId && s.ProblemId == nx.Id);
                        if (sub is not null)
                            nx.IsSubmitted = true;
                    }
                }

                foreach (var t in x.Topics)
                {
                    nx.TopicShortTitle.Add(t.ShortTitle);
                }

                ans.Add(nx);
            }

            return ans;
        }



        public async Task<IEnumerable<Test>> GetTestsAsync(int id)
        {
            var problem = await _context.Problems
                .Include(p => p.Tests)
                .FirstOrDefaultAsync(p => p.Id == id);

            return problem.Tests;
        }

        public async Task<Test>? AddTestAsync(int id, CreateTestDto dto)
        {
            var problem = await _context.Problems
                .Include(p => p.Tests)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (problem is null)
                return null;

            var test = new Test
            {
                Input = dto.Input,
                Output = dto.Output,
                Point = dto.Point,
                ProblemId = problem.Id,
                Problem = problem
            };

            problem.Tests.Add(test);
            await _context.SaveChangesAsync();

            return test;
        }
    }
}
