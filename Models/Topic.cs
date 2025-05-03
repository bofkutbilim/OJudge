namespace OJudge.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; } = null;
        public List<Problem> Problems { get; set; } = new();
    }
}
