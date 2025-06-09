namespace OJudge.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public required string Title { get; set; }

        public string? Description { get; set; } = null;

        public List<User> Users { get; set; } = new List<User>();
    }
}
