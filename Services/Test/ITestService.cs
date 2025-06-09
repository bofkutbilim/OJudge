using OJudge.Dtos;
using OJudge.Models;

namespace OJudge.Services
{
    public interface ITestService
    {
        Task<Test?> DeleteAsync(int id);
        Task<Test?> UpdateAsync(int id, UpdateTestDto dto);
    }
}
