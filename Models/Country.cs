namespace OJudge.Models
{
    public class Country
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public List<User> Users = new List<User>();
    }
}
