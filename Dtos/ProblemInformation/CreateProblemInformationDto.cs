using OJudge.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace OJudge.Dtos
{
    public class CreateProblemInformationDto
    {
        public string? SectionName { get; set; } = null;
        public string? SectionText { get; set; } = null;
    }
}
