using System.ComponentModel.DataAnnotations.Schema;

namespace OJudge.Models
{
    public class ProblemInformation
    {
        public int Id { get; set; }
        public string? SectionName { get; set; } = null;
        public string? SectionText { get; set; } = null;

        [ForeignKey("Problem")]
        public int ProblemId { get; set; }
        public Problem Problem { get; set; }
    }
}
