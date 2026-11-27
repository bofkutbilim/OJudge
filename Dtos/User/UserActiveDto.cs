namespace OJudge.Dtos;

public sealed class UserActiveDto
{
    public string Message { get; init; } = "Active";
    public required int UserId { get; init; }
    public required string UserRole { get; init; }
    public required string UserNickName { get; init; }
}
