namespace OJudge.Dtos
{
    public class UserRanklistDto
    {
        public int Id { get; set; }
        public string NickName {get; set;} = null!;
        public string? Name { get; set; } = null;
        public string? SurName { get; set; } = null;
        public int? OrganizationId { get; set; } = null;
        public string? Organization { get; set; } = null;
        public int Solved { get; set; } = 0;
        public int Submitted { get; set; } = 0;
    }
}
