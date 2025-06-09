namespace OJudge.Models
{
    public class Verdict
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public List<Submission> Submissions = new List<Submission>();
    }
}
