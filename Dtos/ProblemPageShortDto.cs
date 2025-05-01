using OJudge.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace OJudge.Dtos
{
    public class ProblemPageShortDto
    {
        public string? Section { get; set; } = string.Empty;
        public string? TextHtml { get; set; } = string.Empty;
    }
}
