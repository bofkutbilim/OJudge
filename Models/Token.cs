namespace OJudge.Models
{
    public class Token
    {
        public int Id { get; set; }
        //N:1
        public int? UserId { get; set; } = null;
        public User? User { get; set; } = null;
        //
        public string? IpAddress { get; set; } = null;
        public string? MyToken { get; set; } = null;
        public bool? IsActive { get; set; } = true;
    }
}
