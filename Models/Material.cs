using System.ComponentModel.DataAnnotations.Schema;

namespace OJudge.Models
{
    public class Material
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string FilePath {get; set;}
        [ForeignKey("Problem")]
        public int ProblemId;
        public required Problem Problem;
    }
}
