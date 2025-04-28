namespace OJudge.Dtos
{
    public class ProblemWithoutId
    {
        public string Title { get; set; }
        public double? TimeLimitSec { get; set; } = 2;
        public int? MemoryLimitMb { get; set; } = 256;
    }
}
