namespace OJudge.Dtos
{
    public class CreateSubmissionDto
    {
        public required int problemId { get; set; }
        public required string ProgrammingLanguageTitle { get; set; }
        public required IFormFile File { get; set; }
    }
}
