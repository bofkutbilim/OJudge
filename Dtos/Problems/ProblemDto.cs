namespace OJudge.Dtos
{
    public class ProblemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; } = null;
        public double? TimeLimitSec { get; set; } = null;
        public int? MemoryLimitMB { get; set; } = null;
        public List<TopicShortDto>? Topics { get; set; } = null;
        public List<ProblemInformationShortDto>? ProblemInformations { get; set; } = null;
    }
}
