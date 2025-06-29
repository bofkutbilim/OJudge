using System.ComponentModel.DataAnnotations;

namespace OJudge.Dtos
{
    public class UpdateProblemDto
    {
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string? Title { get; set; } = null;

        [Range(1, 20000, ErrorMessage = "Время выполнения должно быть от 1 до 20000 миллисекунд")]
        public int? TimeLimitMilliseconds { get; set; } = null;

        [Range(1, 256000, ErrorMessage = "Память должна быть от 1 до 256000 киллобайт")]
        public int? MemoryLimitKB { get; set; } = null;

        [Range(1, 100, ErrorMessage = "Сложность должны быть от 1 до 100")]
        public int? Difficulty { get; set; } = null;
    }
}
