using Microsoft.AspNetCore.Identity;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Dtos;
using OJudge.Models;

namespace OJudge.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _hasher;

        public UserService(AppDbContext context, IPasswordHasher<User> hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        public async Task<IEnumerable<UserRanklistDto>> GetAllAsync()
        {
            List<UserRanklistDto> ans = new List<UserRanklistDto>();

            foreach (var x in await _context.Users
                .Include(u => u.Organization)
                .OrderByDescending(u => u.Solved)
                .ToListAsync())
            {
                ans.Add(new UserRanklistDto
                {
                    Id = x.Id,
                    NickName = x.NickName,
                    Name = x.Name,
                    SurName = x.Surname,
                    OrganizationId = x.OrganizationId,
                    Organization = (x.Organization is not null ? x.Organization.Title : null),
                    Solved = x.Solved,
                    Submitted = x.Submitted
                });
            }

            return ans;
        }
        public async Task<IEnumerable<UserRanklistDto>> GetTopAsync()
        {
            List<UserRanklistDto> ans = new List<UserRanklistDto>();

            foreach (var x in await _context.Users
                .Include(u => u.Organization)
                .OrderByDescending(u => u.Solved)
                .ToListAsync())
            {
                if (ans.Count() < 5)
                {
                    ans.Add(new UserRanklistDto
                    {
                        Id = x.Id,
                        NickName = x.NickName,
                        Name = x.Name,
                        SurName = x.Surname,
                        OrganizationId = x.OrganizationId,
                        Organization = (x.Organization is not null ? x.Organization.Title : null),
                        Solved = x.Solved,
                        Submitted = x.Submitted
                    });
                }
                else break;
            }

            return ans;
        }
        public async Task<UserProfileDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organization)
                .Include(u => u.Country)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user is null ? null : ToProfile(user);
        }

        private static UserProfileDto ToProfile(User user)
        {
            return new UserProfileDto
            {
                Id = user.Id,
                NickName = user.NickName,
                Name = user.Name,
                SurName = user.Surname,
                Role = user.Role.Title,
                CountryId = user.CountryId,
                CountryTitle = (user.Country is not null ? user.Country.Title : null),
                OrganizationId = user.OrganizationId,
                OrganizationTitle = (user.Organization is not null ? user.Organization.Title : null),
                PhotoPath = user.PhotoPath,
                Solved = user.Solved,
                Submitted = user.Submitted
            };
        }

        public async Task<UserProfileDto?> CreateAsync(CreateUserDto dto, CancellationToken ct)
        {
            if (await _context.Users.AnyAsync(u => u.NickName == dto.NickName, ct))
                return null;

            var user = new User
            {
                NickName = dto.NickName,
                EMail = dto.Email,
                Role = await _context.Roles.SingleAsync(r => r.Title == AuthRoles.User, ct),
                Organization = await _context.Organizations.FindAsync([1], ct),
                Country = await _context.Countries.FindAsync([1], ct)
            };
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);
            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException
                   { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "IX_Users_NickName" })
            {
                return null;
            }

            return ToProfile(user);
        }

        public async Task<UserProfileDto?> UpdateAsync(int id, UpdateUserDto dto, CancellationToken ct)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organization)
                .Include(u => u.Country)
                .FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is null) return null;

            var organization = await _context.Organizations.FindAsync([dto.OrganizationId], ct);
            var country = await _context.Countries.FindAsync([dto.CountryId], ct);
            if (organization is not null) user.Organization = organization;
            if (country is not null) user.Country = country;
            if (dto.Name is not null) user.Name = dto.Name;
            if (dto.SurName is not null) user.Surname = dto.SurName;

            await _context.SaveChangesAsync(ct);
            return ToProfile(user);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var user = await _context.Users.FindAsync([id], ct);
            if (user is null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<User?> AuthenticateAsync(LoginDataDto dto, CancellationToken ct)
        {
            var users = _context.Users.AsNoTracking().Include(u => u.Role);
            var user = await users.SingleOrDefaultAsync(u => u.NickName == dto.NickName, ct);
            if (user is null) return null;

            var result = VerifyPassword(user, dto.Password);
            if (result == PasswordVerificationResult.Failed) return null;
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                // Replace only the verified hash if another request changed credentials in the meantime.
                var newHash = _hasher.HashPassword(user, dto.Password);
                var changed = await _context.Users
                    .Where(u => u.Id == user.Id && u.PasswordHash == user.PasswordHash)
                    .ExecuteUpdateAsync(set => set.SetProperty(u => u.PasswordHash, newHash), ct);
                if (changed == 0)
                {
                    var current = await users.SingleOrDefaultAsync(u => u.Id == user.Id, ct);
                    return current is not null && VerifyPassword(current, dto.Password) != PasswordVerificationResult.Failed
                        ? current : null;
                }
                user.PasswordHash = newHash;
            }
            return user;
        }

        private PasswordVerificationResult VerifyPassword(User user, string password)
        {
            // Only pre-JWT accounts have a decimal hash. Upgrade on their first successful login.
            if (user.PasswordHash.Length is > 0 and <= 10 && user.PasswordHash.All(char.IsAsciiDigit))
                return user.PasswordHash == MyHashService.CreateHash(password)
                    ? PasswordVerificationResult.SuccessRehashNeeded : PasswordVerificationResult.Failed;

            try
            {
                return _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            }
            catch (FormatException)
            {
                return PasswordVerificationResult.Failed;
            }
        }
    }
}
