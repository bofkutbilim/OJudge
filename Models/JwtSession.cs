namespace OJudge.Models;

public sealed class JwtSession
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
