namespace OJudge.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double? TimeLimitSec { get; set; } = 2;
        public int? MemoryLimitMb { get; set; } = 256;

        public List<ProblemPage> ProblemPages { get; set; } = new List<ProblemPage>();
    }
}
