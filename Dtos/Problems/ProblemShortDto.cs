namespace OJudge.Dtos;

public class ProblemShortDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int? Solved { get; set; } = 0;
    public int? Submitted { get; set; } = 0;
    public int? Difficulty { get; set; } = null;
    public List<String> TopicShortTitle { get; set; } = new List<String>();
    public bool? IsSolved { get; set; } = null;
    public bool? IsSubmitted { get; set; } = null;
}
