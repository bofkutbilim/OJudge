namespace OJudge.Dtos;

public sealed class TokenDto
{
    public required string AccessToken { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public string TokenType { get; init; } = "Bearer";
}
