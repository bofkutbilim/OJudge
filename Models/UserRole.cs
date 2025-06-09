namespace OJudge.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //1:N
        public List<User> Users { get; set; } = new List<User>();
    }
}
