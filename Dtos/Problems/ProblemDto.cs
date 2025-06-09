namespace OJudge.Dtos
{
    public class ProblemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; } = null;
        public int? TimeLimitMilliseconds { get; set; } = null;
        public int? MemoryLimitKB { get; set; } = null;
        public int? Solved { get; set; } = null;
        public int? Submitted { get; set; } = null;
        public List<TopicShortDto>? Topics { get; set; } = null;
        public List<ProblemInformationShortDto>? ProblemInformations { get; set; } = null;
    }
}
