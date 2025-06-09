namespace OJudge.Dtos
{
    public class CreateUserDto
    {
        public required string NickName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
