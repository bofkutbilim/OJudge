using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Dtos;
using OJudge.Models;
using OJudge.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace OJudge.Controllers
{

    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [ApiController]
    [Route("api/submission")]
    public class SubmissionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SubmissionController> _logger;
        public SubmissionController(AppDbContext context, ILogger<SubmissionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private Task<Verdict> GetVerdictAsync(string title) =>
            _context.Verdicts.OrderBy(v => v.Id).FirstAsync(v => v.Title == title);

        [HttpPost("submit{problemId}")]
        public async Task<IActionResult> CreateSubmissionAsync([FromRoute] int problemId, [FromForm] CreateSubmissionDto dto,
            [FromQuery(Name = "ProgrammingLanguageTitle")] string? queryLanguage)
        {
            if (dto is null) return BadRequest(new { message = "Введите данные!" });
            if (dto.File is null || dto.File.Length == 0)
                return BadRequest(new { message = "Введите корректный файл!" });

            var problem = await _context.Problems.FindAsync(problemId);
            if (problem is null) return NotFound(new { message = "Такая задача не существует!" });

            if (User.GetUserId() is not int userId) return Unauthorized();
            var user = await _context.Users.FindAsync(userId);
            if (user is null) return Unauthorized();

            var languageTitle = dto.ProgrammingLanguageTitle ?? queryLanguage;
            if (string.IsNullOrWhiteSpace(languageTitle)) return BadRequest(new { message = "Выберите язык программирования!" });
            var programmingLanguage = await _context.ProgrammingLanguages.FirstOrDefaultAsync(pl => pl.Title == languageTitle);
            if (programmingLanguage is null) return NotFound(new { message = "Такого языка программирования не существует!" });

            string code = string.Empty;
            using (var reader = new StreamReader(dto.File.OpenReadStream(), Encoding.UTF8))
            {
                code = await reader.ReadToEndAsync();
            }

            var submission = new Submission
            {
                UserId = user.Id,
                User = user,
                ProblemId = problem.Id,
                Problem = problem,
                ProgrammingLanguageId = programmingLanguage.Id,
                ProgrammingLanguage = programmingLanguage,
                Verdict = await GetVerdictAsync(programmingLanguage.Title.Contains("C++")
                    ? Verdict.Compiling : Verdict.UnsupportedLanguage),
                CodeText = code
            };
            await _context.Submissions.AddAsync(submission);
            user.Submitted++;
            problem.Submitted++;
            await _context.SaveChangesAsync();

            if (submission.Verdict.Title == Verdict.Compiling)
            {
                await JudgeCppAsync(submission);
            }

            return Ok(new { id = submission.Id, message = "Отправлена успешно!" });
        }

        private async Task JudgeCppAsync(Submission submission)
        {
            DirectoryInfo? workspace = null;
            try
            {
                workspace = Directory.CreateTempSubdirectory("ojudge-");
                var exePath = Path.Combine(workspace.FullName, "program.exe");
                var compilerError = await CppCompilerService.Compile(submission.CodeText, exePath);
                if (compilerError is not null)
                {
                    submission.CompilerAnswer = compilerError;
                    submission.Verdict = await GetVerdictAsync(Verdict.CompileError);
                    await _context.SaveChangesAsync();
                    return;
                }

                submission.Verdict = await GetVerdictAsync(Verdict.Testing);
                await _context.SaveChangesAsync();
                var tests = await _context.Tests
                    .Where(t => t.ProblemId == submission.ProblemId)
                    .OrderBy(t => t.Id)
                    .ToListAsync();

                int totalPoints = 0;

                int idx = 1;
                foreach (var test in tests)
                {
                    var startInfo = new ProcessStartInfo(exePath)
                    {
                        WorkingDirectory = workspace.FullName,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    JudgeProcessEnvironment.LimitToToolchain(startInfo);
                    using var process = Process.Start(startInfo) ?? throw new IOException("Could not start the submission.");

                    var output = process.StandardOutput.ReadToEndAsync();
                    try
                    {
                        await process.StandardInput.WriteAsync(test.Input);
                        process.StandardInput.Close();
                    }
                    catch (IOException)
                    {
                        // A submission may exit or close stdin before consuming the entire test.
                    }
                    string result = await output;
                    await process.WaitForExitAsync();

                    // Сравниваем результат
                    var expected = test.Output.Trim();
                    var actual = result.Trim();

                    if (process.ExitCode == 0 && expected == actual)
                    {
                        if (test.Point is not null)
                            totalPoints += (int)test.Point;
                    }
                    else
                    {
                        submission.LastTest = idx;
                        submission.Points = totalPoints;
                        submission.Verdict = await GetVerdictAsync(process.ExitCode == 0
                            ? Verdict.WrongAnswer : Verdict.RuntimeError);
                        await _context.SaveChangesAsync();
                        return;
                    }

                    idx++;
                }

                if (tests.Count == 0)
                {
                    submission.Verdict = await GetVerdictAsync(Verdict.JudgeError);
                    submission.CompilerAnswer = "У задачи нет тестов.";
                }
                else
                {
                    submission.Points = totalPoints;
                    submission.LastTest = tests.Count;
                    if (!await _context.Submissions.AnyAsync(s => s.ProblemId == submission.ProblemId &&
                        s.UserId == submission.UserId && s.Verdict.Title == Verdict.Accepted))
                    {
                        submission.User.Solved++;
                        submission.Problem.Solved++;
                    }
                    submission.Verdict = await GetVerdictAsync(Verdict.Accepted);
                }
            }
            catch (Exception ex) when (ex is Win32Exception or IOException or UnauthorizedAccessException)
            {
                _logger.LogError(ex, "Could not judge submission {SubmissionId}", submission.Id);
                submission.Verdict = await GetVerdictAsync(Verdict.JudgeError);
                submission.CompilerAnswer = "Не удалось выполнить проверку на сервере.";
            }
            finally
            {
                try
                {
                    workspace?.Delete(recursive: true);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    _logger.LogWarning(ex, "Could not clean up files for submission {SubmissionId}", submission.Id);
                }
            }
            await _context.SaveChangesAsync();
        }


        private IQueryable<Submission> SubmissionsWithDetailsAccess()
        {
            var submissions = _context.Submissions.AsNoTracking();
            if (User.IsInRole(AuthRoles.Admin)) return submissions;

            var userId = User.GetUserId();
            return submissions.Where(s => s.UserId == userId);
        }

        private static IQueryable<SubmissionDetailsDto> ProjectSubmissions(IQueryable<Submission> submissions) =>
            submissions.OrderByDescending(s => s.Id).Select(s => new SubmissionDetailsDto
            {
                Summary = new SubmissionListDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    UserNickName = s.User.NickName,
                    ProblemId = s.ProblemId,
                    ProblemTitle = s.Problem.Title,
                    VerdictTitle = s.Verdict.Title,
                    Points = s.Points,
                    LastTest = s.LastTest,
                    TimeLimitMilliseconds = s.TimeLimitMilliseconds,
                    MemoryLimitKB = s.MemoryLimitKB,
                    ProgrammingLanguageTitle = s.ProgrammingLanguage.Title,
                    Received = s.Received
                },
                CodeText = s.CodeText,
                CompilerError = s.CompilerAnswer
            });

        [AllowAnonymous]
        [HttpGet("get")]
        [HttpGet("get/filter")]
        public Task<List<SubmissionListDto>> GetAllAsync([FromQuery] string? problem, [FromQuery] string? verdict,
            [FromQuery] string? language, [FromQuery] string? user, CancellationToken ct)
        {
            var query = _context.Submissions.AsNoTracking();
            if (!string.IsNullOrEmpty(problem)) query = query.Where(s => s.Problem.Title.Contains(problem));
            if (!string.IsNullOrEmpty(user)) query = query.Where(s => s.User.NickName.Contains(user));
            if (!string.IsNullOrEmpty(verdict) && verdict != "none") query = query.Where(s => s.Verdict.Title == verdict);
            if (!string.IsNullOrEmpty(language) && language != "none") query = query.Where(s => s.ProgrammingLanguage.Title == language);

            return ProjectSubmissions(query).Select(row => row.Summary).ToListAsync(ct);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SubmissionDetailsDto>> GetByIdAsync(int id, CancellationToken ct)
        {
            var item = await ProjectSubmissions(SubmissionsWithDetailsAccess().Where(s => s.Id == id)).SingleOrDefaultAsync(ct);
            if (item is null) return NotFound(new { message = "Посылка не найдена." });
            return Ok(item);
        }

        [HttpGet("getmy{problemId}")]
        public Task<List<SubmissionListDto>> GetMyAsync(int problemId, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var query = _context.Submissions.AsNoTracking().Where(s => s.UserId == userId && s.ProblemId == problemId);
            return ProjectSubmissions(query).Select(row => row.Summary).ToListAsync(ct);
        }
    }
}
