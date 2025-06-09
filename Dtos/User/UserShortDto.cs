namespace OJudge.Dtos
{
    public class UserShortDto
    {
        public int UserId { get; set; }
        public string EMail { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
