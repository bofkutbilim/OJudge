namespace OJudge.Dtos
{
    public class UpdateProblemDto
    {
        public string? Title { get; set; } = null;
        public double? TimeLimitSec { get; set; } = null;
        public int? MemoryLimitMB { get; set; } = null;
    }
}
