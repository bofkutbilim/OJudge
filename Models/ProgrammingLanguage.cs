namespace OJudge.Models
{
    public class ProgrammingLanguage
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public List<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
