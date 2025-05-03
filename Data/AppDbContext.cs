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
    }
}