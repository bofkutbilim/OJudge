using System.ComponentModel.DataAnnotations.Schema;


namespace OJudge.Models
{
    public class Test
    {
        public int Id { get; set; }
        public required string Input { get; set; }
        public required string Output { get; set; }
        public int? Point { get; set; } = 0;
        //N:1

        [ForeignKey("Problem")]
        public int ProblemId { get; set; }
        public required Problem Problem { get; set; }
    }
}
