using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJudge.Data;
using OJudge.Dtos;
using OJudge.Models;

namespace OJudge.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context) {
            _context = context;
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

            if (user is null)
                return null;

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
        public async Task<User>? CreateAsync(CreateUserDto dto)
        {
            if (await _context.Users
                .FirstOrDefaultAsync(u => u.NickName == dto.NickName) is not null)
                return null;

            var user = new User
            {
                NickName = dto.NickName,
                RoleId = 1,
                Role = await _context.Roles.FindAsync(1),
                Organization = await _context.Organizations.FindAsync(1),
                Country = await _context.Countries.FindAsync(1),
                Solved = 0,
                Submitted = 0,
                EMail = dto.Email,
                PasswordHash = MyHashService.CreateHash(dto.Password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Organization)
                .Include(u => u.Country)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return null;

            var organization = await _context.Organizations.FindAsync(dto.OrganizationId);
            var country = await _context.Countries.FindAsync(dto.CountryId);

            if (organization is not null)
            {
                user.Organization = organization;
                user.OrganizationId = organization.Id;
            }

            if (country is not null)
            {
                user.Country = country;
                user.CountryId = country.Id;
            }

            if (dto.Name is not null) user.Name = dto.Name;
            if (dto.SurName is not null) user.Surname = dto.SurName;
            
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
                return null;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<User?> LoginAsync(LoginDataDto dto, string? ip)
        {
            var getHash = MyHashService.CreateHash(dto.Password);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.NickName == dto.NickName && u.PasswordHash == getHash);

            if (user is null)
                return null;

            var token = await _context.Tokens.FirstOrDefaultAsync(t => t.UserId == user.Id && t.IpAddress == ip);

            if (token is null)
            {
                token = new Token
                {
                    UserId = user.Id,
                    IpAddress = ip,
                    MyToken = MyTokenService.GenerateToken()
                };

                await _context.Tokens.AddAsync(token);
                await _context.SaveChangesAsync();
            }
            else if (token.IsActive == false)
            {
                token.IsActive = true;
                await _context.SaveChangesAsync();
            }

            return user;
        }
        public async Task<User?> LogoutAsync(string? ip)
        {

            var token = await _context.Tokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.IpAddress == ip && t.IsActive == true);

            if (token is null)
                return null;

            token.IsActive = false;
            await _context.SaveChangesAsync();

            return token.User;
        }
        public async Task<UserActiveDto> CheckActiveAsync(string? ip)
        {
            var token = await _context.Tokens
                .Include(t => t.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(t => t.IpAddress == ip && t.IsActive == true);

            if (token is null || token.IsActive == false)
                return null;

            if (token is not null && token.IsActive == true)
            {
                return new UserActiveDto {
                    message = "Active",
                    userId = (int)token.UserId,
                    userRole = token.User?.Role?.Title,
                    userNickName = token.User.NickName
                };
            }

            return null;
        }
    }
}
