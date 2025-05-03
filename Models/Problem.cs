namespace OJudge.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public double? TimeLimitSec { get; set; } = 2;
        public int? MemoryLimitMB { get; set; } = 256;

        public List<ProblemInformation> ProblemInformations { get; set; } = new List<ProblemInformation>();
        public List<Topic> Topics { get; set; } = new();
    }
}
