namespace OJudge.Dtos
{
    public class CreateSubmissionDto
    {
        public string? ProgrammingLanguageTitle { get; set; }
        public required IFormFile File { get; set; }
    }
}
