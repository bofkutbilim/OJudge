using System.ComponentModel.DataAnnotations.Schema;

namespace OJudge.Models
{
    public class Submission
    {
        public int Id { get; set; }
        //N:1
        [ForeignKey("User")]
        public int UserId { get; set; }
        public required User User { get; set; }
        //N:1
        [ForeignKey("Problem")]
        public int ProblemId { get; set; }
        public required Problem Problem { get; set; }
        //N:1
        [ForeignKey("Verdict")]
        public int VerdictId {get; set; }
        public required Verdict Verdict { get; set; }
        //
        public int? Points { get; set; } = 0;
        public int? LastTest { get; set; } = 0;
        public int? TimeLimitMilliseconds { get; set; } = 0;
        public int? MemoryLimitKB { get; set; } = 0;
        //N:1
        [ForeignKey("ProgrammingLanguage")]
        public int ProgrammingLanguageId { get; set; }
        public required ProgrammingLanguage ProgrammingLanguage { get; set; }
        //
        public DateTime? Received { get; set; } = DateTime.UtcNow;
        public required string CodeText { get; set; }
        public string? CompilerAnswer { get; set; } = null;
    }
}
