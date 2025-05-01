using System.ComponentModel.DataAnnotations.Schema;

namespace OJudge.Models
{
    public class ProblemPage
    {
        public int Id { get; set; }
        public string? Section { get; set; } = string.Empty;
        public string? TextHtml { get; set; } = string.Empty;

        [ForeignKey("Problem")]
        public int ProblemId { get; set; }
        public Problem Problem { get; set; }
    }
}
