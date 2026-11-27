using OJudge.Dtos;
using OJudge.Models;

namespace OJudge.Services;

public interface IUserService
{
    Task<IEnumerable<UserRanklistDto>> GetAllAsync();
    Task<IEnumerable<UserRanklistDto>> GetTopAsync();
    Task<UserProfileDto?> GetByIdAsync(int id);
    Task<UserProfileDto?> CreateAsync(CreateUserDto dto, CancellationToken ct);
    Task<UserProfileDto?> UpdateAsync(int id, UpdateUserDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
    Task<User?> AuthenticateAsync(LoginDataDto dto, CancellationToken ct);
}
