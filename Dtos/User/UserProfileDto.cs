namespace OJudge.Dtos
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Role { get; set; } = null!;
        public string NickName { get; set; } = null!;
        public string? Name { get; set; } = null;
        public string? SurName { get; set; } = null;
        public int? CountryId { get; set; } = null;
        public string? CountryTitle { get; set; } = null;
        public int? OrganizationId { get; set; } = null;
        public string? OrganizationTitle { get; set; } = null;
        public int Solved { get; set; } = 0;
        public int Submitted { get; set; } = 0;
        public string? PhotoPath { get; set; } = null;
    }
}
