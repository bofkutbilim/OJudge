namespace OJudge.Dtos
{
    public class MainTopicDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public List<TopicListDto> Topics { get; set; } = new List<TopicListDto>();
    }
}
