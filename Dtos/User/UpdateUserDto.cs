namespace OJudge.Dtos
{
    public class UpdateUserDto
    {
        public string Name { get; set; } = null;
        public string SurName { get; set; } = null;
        public int CountryId { get; set; }
        public int OrganizationId { get; set; }
    }
}
