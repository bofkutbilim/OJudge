using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OJudge.Models;

namespace OJudge.Data;

internal static class DatabaseInitializer
{
    private sealed record Account(string NickName, string Email, string? PasswordHash);

    internal static async Task SeedAsync(AppDbContext db, IHostEnvironment environment,
        IConfiguration configuration, IPasswordHasher<User> hasher, ILogger logger)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        // Serialize the existence checks with other instances and concurrent catalog/account writes.
        await db.Database.ExecuteSqlRawAsync("""
            LOCK TABLE "Roles", "Countries", "Organizations", "ProgrammingLanguages", "Verdicts", "Users"
            IN SHARE ROW EXCLUSIVE MODE
            """);

        await AddMissingTitlesAsync(db.Roles, title => new UserRole { Title = title },
            AuthRoles.User, AuthRoles.Admin);
        await AddMissingTitlesAsync(db.ProgrammingLanguages, title => new ProgrammingLanguage { Title = title },
            "C++ 14 (64x)", "Python 3.10", "Java 11");
        await AddMissingTitlesAsync(db.Verdicts, title => new Verdict { Title = title },
            Verdict.Compiling, Verdict.Testing, Verdict.CompileError, Verdict.WrongAnswer, Verdict.Accepted,
            Verdict.UnsupportedLanguage, Verdict.RuntimeError, Verdict.JudgeError,
            "Time Limit", "Memory Limit", "Presentation Error");
        await AddMissingTitlesAsync(db.Countries, title => new Country { Title = title }, "Кыргызстан");
        await AddMissingTitlesAsync(db.Organizations, title => new Organization { Title = title },
            "Кут билим окуу-тарбия комплекси", "Чөгөм орто мектеби");
        if (db.ChangeTracker.Entries<Verdict>().Any(entry => entry.State == EntityState.Added))
        {
            // Older databases may have verdicts inserted with explicit IDs ahead of the identity sequence.
            await db.Database.ExecuteSqlRawAsync("""
                SELECT setval(seq, greatest(nextval(seq), (SELECT coalesce(max("Id")::bigint, 0) + 1 FROM "Verdicts")), false)
                FROM (SELECT pg_get_serial_sequence('"Verdicts"', 'Id')::regclass AS seq) AS verdict_sequence
                """);
        }
        await db.SaveChangesAsync();

        var account = await ReadAccountAsync(environment, configuration);
        await AddAdminAsync(db, account, configuration["BootstrapAdmin:Password"], hasher, logger);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private static async Task AddMissingTitlesAsync<TEntity>(DbSet<TEntity> rows,
        Func<string, TEntity> create, params string[] titles) where TEntity : class
    {
        var existing = await rows.Select(row => EF.Property<string>(row, "Title"))
            .Where(title => titles.Contains(title)).ToListAsync();
        rows.AddRange(titles.Except(existing, StringComparer.Ordinal).Select(create));
    }

    private static async Task<Account> ReadAccountAsync(IHostEnvironment environment, IConfiguration configuration)
    {
        if (environment.IsDevelopment())
        {
            var path = Path.Combine(environment.ContentRootPath, ".local", "seed-admin.json");
            if (File.Exists(path))
            {
                using var input = File.OpenRead(path);
                var local = await JsonSerializer.DeserializeAsync<Account>(input);
                if (local is null || string.IsNullOrWhiteSpace(local.NickName) ||
                    string.IsNullOrWhiteSpace(local.Email) || string.IsNullOrWhiteSpace(local.PasswordHash))
                    throw new InvalidDataException("Incomplete development admin configuration.");
                return local;
            }
        }

        return new Account("admin", "erjanulukbekov04@gmail.com",
            environment.IsDevelopment() ? configuration["BootstrapAdmin:DevelopmentPasswordHash"] : null);
    }

    private static async Task AddAdminAsync(AppDbContext db, Account seed, string? password,
        IPasswordHasher<User> hasher, ILogger logger)
    {
        var existing = await db.Users.Where(u => u.NickName == seed.NickName || u.EMail == seed.Email)
            .Select(u => new { u.NickName, u.EMail, Role = u.Role.Title }).ToListAsync();
        if (existing.Count != 0)
        {
            if (!existing.Any(u => u.NickName == seed.NickName && u.EMail == seed.Email && u.Role == AuthRoles.Admin))
                logger.LogWarning("Bootstrap admin was not created: the nickname or email is already in use.");
            return;
        }

        var admin = new User
        {
            NickName = seed.NickName,
            Name = seed.NickName,
            EMail = seed.Email,
            RoleId = await db.Roles.Where(r => r.Title == AuthRoles.Admin).Select(r => r.Id).SingleAsync()
        };
        admin.PasswordHash = !string.IsNullOrWhiteSpace(password) ? hasher.HashPassword(admin, password)
            : seed.PasswordHash ?? throw new InvalidOperationException(
                "Set BootstrapAdmin__Password for the first startup with a new admin account.");
        db.Users.Add(admin);
    }
}
