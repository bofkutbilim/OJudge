namespace OJudge.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int? TimeLimiMilliseconds { get; set; } = 2;
        public int? MemoryLimitKB { get; set; } = 256;
        public int? Difficulty { get; set; } = null;
        public int? Solved { get; set; } = 0;
        public int? Submitted { get; set; } = 0;
        //1:N
        public List<ProblemInformation> ProblemInformations { get; set; } = new List<ProblemInformation>();
        public List<Submission> Submissions { get; set; } = new List<Submission>();
        //N:N
        public List<Topic> Topics { get; set; } = new();
        //1:N
        public List<Test> Tests { get; set; } = new();
    }
}
