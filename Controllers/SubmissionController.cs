using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Dtos;
using OJudge.Models;
using OJudge.Services;
using System.Diagnostics;
using System.Text;

namespace OJudge.Controllers
{

    [ApiController]
    [Route("api/submission")]
    public class SubmissionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SubmissionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("submit{problemId}")]
        public async Task<IActionResult> CreateSubmissionAsync(int problemId, CreateSubmissionDto dto)
        {
            if (dto is null) return BadRequest(new { message = "Введите данные!" });
            if (dto.File is null || dto.File.Length == 0)
                return BadRequest(new { message = "Введите корректный файл!" });

            var problem = await _context.Problems.FindAsync(problemId);
            if (problem is null) return NotFound(new { message = "Такая задача не существует!" });

            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var token = await _context.Tokens.Include(t => t.User).ThenInclude(u => u.Role).FirstOrDefaultAsync(t => t.IpAddress == ip && t.IsActive == true);
            if (token is not null && token.IsActive == true)
            {
                var user = token.User;
                if (user is null) return NotFound(new { message = "Войдите в систему!" });

                var programmingLanguage = await _context.ProgrammingLanguages.FirstOrDefaultAsync(pl => pl.Title == dto.ProgrammingLanguageTitle);
                if (programmingLanguage is null) return NotFound(new { message = "Такого языка программирования не существует!" });

                string code = string.Empty;
                using (var reader = new StreamReader(dto.File.OpenReadStream(), Encoding.UTF8))
                {
                    code = reader.ReadToEnd();
                }

                var submission = new Submission
                {
                    UserId = user.Id,
                    User = user,
                    ProblemId = problem.Id,
                    Problem = problem,
                    ProgrammingLanguageId = programmingLanguage.Id,
                    ProgrammingLanguage = programmingLanguage,
                    VerdictId = 1,
                    Verdict = _context.Verdicts.Find(3),
                    CodeText = code
                };
                await _context.Submissions.AddAsync(submission);
                user.Submitted++;
                problem.Submitted++;
                await _context.SaveChangesAsync();

                string compilerAnswer = (programmingLanguage.Title.Contains("C++") ? await CppCompilerService.Compile(code) : "no cpp");

                if (compilerAnswer == "no cpp")
                {
                    submission.Verdict = _context.Verdicts.Find(13);
                }
                if (compilerAnswer == "ok")
                {
                    submission.Verdict = _context.Verdicts.Find(6);
                }
                else
                {
                    submission.CompilerAnswer = compilerAnswer;
                    submission.Verdict = _context.Verdicts.Find(4);
                }
                await _context.SaveChangesAsync();

                if (compilerAnswer == "ok")
                {
                    var tests = await _context.Tests
                        .Where(t => t.ProblemId == problemId)
                        .ToListAsync();

                    int totalPoints = 0;
                    bool wrongAnswer = false;
                    string exePath = @"C:\\Users\\Amin Stors\\Documents\\AAA Учёба\\Мои проекты\\OJudge\\Codes\\program.exe";

                    int idx = 1;
                    foreach (var test in tests)
                    {
                        var startInfo = new ProcessStartInfo(exePath) // путь к program.exe
                        {
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        using var process = Process.Start(startInfo);

                        // Передаём input программе
                        await process.StandardInput.WriteAsync(test.Input);
                        process.StandardInput.Close(); // обязательно, чтобы программа знала, что ввод завершён

                        // Читаем вывод программы
                        string result = await process.StandardOutput.ReadToEndAsync();
                        await process.WaitForExitAsync();

                        // Сравниваем результат
                        var expected = test.Output.Trim();
                        var actual = result.Trim();

                        if (expected == actual)
                        {
                            if (test.Point is not null)
                                totalPoints += (int)test.Point;
                        }
                        else
                        {
                            submission.LastTest = idx;
                            submission.Points += totalPoints;
                            wrongAnswer = true;
                            break;
                        }

                        idx++;
                        // Можешь добавить лог или вернуть результаты по каждому тесту
                    }

                    if (wrongAnswer == true)
                    {
                        submission.Verdict = _context.Verdicts.Find(7);
                    }
                    else
                    {
                        submission.Points = totalPoints;
                        submission.LastTest = tests.Count;
                        if (await _context.Submissions
                            .FirstOrDefaultAsync(s => s.ProblemId == problem.Id && s.UserId == user.Id && s.VerdictId == 8) is null)
                        {
                            user.Solved++;
                            problem.Solved++;
                        }
                        submission.Verdict = _context.Verdicts.Find(8);
                    }

                    await _context.SaveChangesAsync();
                }

                return Ok(new { id = submission.Id, message = "Отправлена успешно!" });
            }

            return BadRequest(new { message = "Войдите в систему!" });

        }


        [HttpGet("get")]
        public async Task<IEnumerable<SubmissionListDto>> GetAllAsync()
        {
            var ans = new List<SubmissionListDto>();

            var submissions = await _context.Submissions
                .Include(s => s.Problem)
                .Include(s => s.ProgrammingLanguage)
                .Include(s => s.User)
                .OrderDescending()
                .Include(s => s.Verdict)
                .ToListAsync();

            foreach (var x in submissions)
            {
                var nx = new SubmissionListDto
                {
                    Id = x.Id,
                    UserId = x.User.Id,
                    UserNickName = x.User.NickName,
                    ProblemId = x.Problem.Id,
                    ProblemTitle = x.Problem.Title,
                    VerdictTitle = x.Verdict.Title,
                    Points = x.Points,
                    LastTest = x.LastTest,
                    TimeLimitMilliseconds = x.TimeLimitMilliseconds,
                    MemoryLimitKB = x.MemoryLimitKB,
                    ProgrammingLanguageTitle = x.ProgrammingLanguage.Title,
                    Received = x.Received
                };
                ans.Add(nx);
            }

            return ans;
        }

        [HttpGet("get/filter")]
        public async Task<IEnumerable<SubmissionListDto>> GetAllByFilterAsync([FromQuery] string? problem, string? verdict, string? language, string? user)
        {
            if (problem is null) problem = "";
            if (verdict is null) verdict = "";
            if (language is null) language = "";
            if (user is null) user = "";
            
            var ans = new List<SubmissionListDto>();

            var submissions = await _context.Submissions
                .Include(s => s.Problem)
                .Include(s => s.ProgrammingLanguage)
                .Include(s => s.User)
                .Include(s => s.Verdict)
                .OrderDescending()
                .ToListAsync();

            foreach (var x in submissions)
            {
                var nx = new SubmissionListDto
                {
                    Id = x.Id,
                    UserId = x.User.Id,
                    UserNickName = x.User.NickName,
                    ProblemId = x.Problem.Id,
                    ProblemTitle = x.Problem.Title,
                    VerdictTitle = x.Verdict.Title,
                    Points = x.Points,
                    LastTest = x.LastTest,
                    TimeLimitMilliseconds = x.TimeLimitMilliseconds,
                    MemoryLimitKB = x.MemoryLimitKB,
                    ProgrammingLanguageTitle = x.ProgrammingLanguage.Title,
                    Received = x.Received
                };
                if (problem == "" || x.Problem.Title.Contains(problem) == true)
                    if (verdict == "" || verdict == "none" || x.Verdict.Title == verdict)
                        if (language == "" || language == "none" || x.ProgrammingLanguage.Title == language)
                            if (user == "" || x.User.NickName.Contains(user) == true)
                                ans.Add(nx);
            }

            return ans;
        }

        [HttpGet("getmy{problemId}")]
        public async Task<IEnumerable<SubmissionListDto>> GetMyAsync(int problemId)
        {
            var ans = new List<SubmissionListDto>();
            
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var token = await _context.Tokens.Include(t => t.User).ThenInclude(u => u.Role).FirstOrDefaultAsync(t => t.IpAddress == ip && t.IsActive == true);

            if (token is not null && token.IsActive == true)
            {
                var submissions = await _context.Submissions
                    .Include(s => s.Problem)
                    .Include(s => s.ProgrammingLanguage)
                    .Include(s => s.User)
                    .Include(s => s.Verdict)
                    .ToListAsync();

                foreach (var x in submissions)
                {

                    if (x.UserId != token.UserId || x.ProblemId != problemId) continue;

                    var nx = new SubmissionListDto
                    {
                        Id = x.Id,
                        UserId = x.User.Id,
                        UserNickName = x.User.NickName,
                        ProblemId = x.Problem.Id,
                        ProblemTitle = x.Problem.Title,
                        VerdictTitle = x.Verdict.Title,
                        Points = x.Points,
                        LastTest = x.LastTest,
                        TimeLimitMilliseconds = x.TimeLimitMilliseconds,
                        MemoryLimitKB = x.MemoryLimitKB,
                        ProgrammingLanguageTitle = x.ProgrammingLanguage.Title,
                        Received = x.Received
                    };
                    ans.Add(nx);
                }

                ans.Reverse();
            }

            return ans;
        }
    }
}
