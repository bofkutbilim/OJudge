namespace OJudge.Dtos
{
    public class SubmissionListDto
    {
        public int? Id { get; set; } = null!;
        public int? UserId { get; set; } = null!;
        public string UserNickName { get; set; } = null!;
        public int? ProblemId { get; set; } = null!;
        public string ProblemTitle { get; set; } = null!;
        public string VerdictTitle { get; set; } = null!;
        public int? Points { get; set; } = 0;
        public int? LastTest { get; set; } = 0;
        public int? TimeLimitMilliseconds { get; set; } = 0;
        public int? MemoryLimitKB { get; set; } = 0;
        public string ProgrammingLanguageTitle { get; set; } = null!;
        public DateTime? Received { get; set; } = null!;
    }
}
