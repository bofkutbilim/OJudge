using System.ComponentModel.DataAnnotations.Schema;

namespace OJudge.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string ShortTitle { get; set; }
        public string? Description { get; set; } = null;
        //N:N
        public List<Problem> Problems { get; set; } = new();
        //N:1
        [ForeignKey("MainTopic")]
        public int MainTopicId;
        public required MainTopic MainTopic { get; set; }
    }
}
