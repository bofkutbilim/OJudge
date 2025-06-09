namespace OJudge.Dtos
{
    public class CreateTestDto
    {
        public required string Input { get; set; }
        public required string Output { get; set; }
        public int Point { get; set; } = 0;
    }
}
