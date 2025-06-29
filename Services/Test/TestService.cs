using OJudge.Data;
using OJudge.Dtos;
using OJudge.Models;

namespace OJudge.Services
{
    public class TestService : ITestService
    {
        private readonly AppDbContext _context;
        public TestService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Test?> DeleteAsync(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test is null)
                return null;

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();

            return test;
        }
        public async Task<Test?> UpdateAsync(int id, UpdateTestDto dto)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test is null)
                return test;

            if (dto.Input is not null)
                test.Input = dto.Input;
            if (dto.Output is not null)
                test.Output = dto.Output;
            if (dto.Point is not null)
                test.Point = dto.Point;

            await _context.SaveChangesAsync();
            
            return test;
        }
    }
}
