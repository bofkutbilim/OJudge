namespace OJudge.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; } = string.Empty;
        public List<Problem> Problems { get; set; } = new();
    }
}
