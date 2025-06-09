using Microsoft.AspNetCore.Mvc;
using OJudge.Dtos;
using OJudge.Models;

namespace OJudge.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<UserRanklistDto>> GetAllAsync();
        public Task<IEnumerable<UserRanklistDto>> GetTopAsync();
        public Task<UserProfileDto>? GetByIdAsync(int id);
        public Task<User?> CreateAsync(CreateUserDto dto);
        public Task<User?> UpdateAsync(int id, UpdateUserDto dto);
        public Task<User?> DeleteAsync(int id);
        public Task<User?> LoginAsync(LoginDataDto dto, string? ip);
        public Task<User?> LogoutAsync(string? ip);
        public Task<UserActiveDto?> CheckActiveAsync(string? ip);
    }
}
