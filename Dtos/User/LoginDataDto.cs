using System.ComponentModel.DataAnnotations;

namespace OJudge.Dtos
{
    public class LoginDataDto
    {
        [Required(ErrorMessage = "Введите никнейм!")]
        [StringLength(50, ErrorMessage = "Никнейм не должно превышать 50 символов")]
        public required string NickName { get; set; }
        [Required(ErrorMessage = "Введите пароль!")]
        [StringLength(50, ErrorMessage = "Пароль не должно превышать 50 символов")]
        public required string Password { get; set; }
    }
}
