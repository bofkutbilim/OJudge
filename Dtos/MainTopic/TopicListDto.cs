namespace OJudge.Dtos
{
    public class TopicListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int? Undefined { get; set; } = 0;
        public int? Easy { get; set; } = 0;
        public int? Medium { get; set; } = 0;
        public int? Hard { get; set; } = 0;
    }
}
