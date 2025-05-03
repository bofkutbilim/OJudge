namespace OJudge.Dtos
{
    public class CreateProblemDto
    {
        public required string Title { get; set; }
        public double? TimeLimitSec { get; set; } = 2;
        public int? MemoryLimitMB { get; set; } = 256;
    }
}
