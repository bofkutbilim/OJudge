namespace OJudge.Models
{
    public class MainTopic
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public List<Topic> Topics { get; set; } = new List<Topic>();
    }
}
