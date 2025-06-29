using System.ComponentModel.DataAnnotations;

namespace OJudge.Dtos
{
    public class CreateProblemDto
    {
        [Required(ErrorMessage = "Введите название!")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public required string Title { get; set; }
        
        [Range(1, 20000, ErrorMessage = "Время выполнения должно быть от 1 до 20000 миллисекунд")]
        public int? TimeLimitMilliseconds { get; set; } = 2000;

        [Range(1, 256000, ErrorMessage = "Память должна быть от 1 до 256000 киллобайт")]
        public int? MemoryLimitKB { get; set; } = 256000;
    }
}
