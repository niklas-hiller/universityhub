using University.Server.Domain.Models;

namespace University.Server.Domain.Services
{
    public interface ISemesterService
    {
        Task<Semester> SaveAsync(Semester semester);
        Task<Semester> CalculateAsync(Guid id);
        Task<IEnumerable<Semester>> GetManyAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<Semester>> GetManyAsyncByTime(DateTime containsDate, TimeSpan? delta = null);
        Task<Semester?> GetAsyncNullable(Guid id);
        Task<Semester> GetAsync(Guid id);
        Task<IEnumerable<Semester>> ListAsync();
        Task<Semester> PatchModulesAsync(Guid id, PatchModel<Module> patch);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<SemesterModule>> GetActiveSemesterModulesOfUser(Guid id);
    }
}
