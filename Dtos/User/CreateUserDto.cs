using System.ComponentModel.DataAnnotations;

namespace OJudge.Dtos;

public sealed class CreateUserDto
{
    [Required, StringLength(50)]
    public required string NickName { get; set; }

    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required, StringLength(50)]
    public required string Password { get; set; }
}
