namespace OJudge.Models
{
    public class Verdict
    {
        public const string Compiling = "Compiling";
        public const string Testing = "Testing";
        public const string CompileError = "Compile Error";
        public const string WrongAnswer = "Wrong Answer";
        public const string Accepted = "Accepted";
        public const string UnsupportedLanguage = "Unsupported Language";
        public const string RuntimeError = "Runtime Error";
        public const string JudgeError = "Judge Error";

        public int Id { get; set; }
        public required string Title { get; set; }
        public List<Submission> Submissions = new List<Submission>();
    }
}
