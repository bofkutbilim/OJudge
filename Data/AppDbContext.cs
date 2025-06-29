using Microsoft.EntityFrameworkCore;
using OJudge.Models;

namespace OJudge.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<ProblemInformation> ProblemInformations { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<MainTopic> MainTopics { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Verdict> Verdicts { get; set; }
        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Material> Materials { get; set; }

        public DbSet<UserRole> Roles { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }
}