using System.ComponentModel.DataAnnotations.Schema;

namespace OJudge.Models
{
    public class ProblemInformation
    {
        public int Id { get; set; }
        public string? SectionName { get; set; } = null;
        public string? SectionText { get; set; } = null;
        
        //N:1

        [ForeignKey("Problem")]
        public int ProblemId { get; set; }
        public required Problem Problem { get; set; }
    }
}
