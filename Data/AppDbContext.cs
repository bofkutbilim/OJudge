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
        public DbSet<JwtSession> JwtSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(user => user.NickName).IsUnique();
            modelBuilder.Entity<User>().HasOne(user => user.Role).WithMany(role => role.Users)
                .HasForeignKey(user => user.RoleId).OnDelete(DeleteBehavior.Restrict);

            // Old installations may have additional, unused role names.
            modelBuilder.Entity<UserRole>().HasIndex(role => role.Title).IsUnique()
                .HasFilter("\"Title\" IN ('admin', 'user')");
            modelBuilder.Entity<JwtSession>().Property(session => session.Id).ValueGeneratedNever();
            modelBuilder.Entity<JwtSession>().HasIndex(session => session.ExpiresAt);
            modelBuilder.Entity<JwtSession>().HasOne(session => session.User).WithMany()
                .HasForeignKey(session => session.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
