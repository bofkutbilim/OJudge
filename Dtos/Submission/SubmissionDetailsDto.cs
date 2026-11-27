namespace OJudge.Dtos;

public sealed class SubmissionDetailsDto
{
    public required SubmissionListDto Summary { get; init; }
    public required string CodeText { get; init; }
}
