using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface ISemesterService
    {
        Task<SemesterResponse> SaveAsync(Semester semester);
        Task<Semester?> GetAsync(Guid id);
        Task<IEnumerable<Semester>> ListAsync();
        Task<SemesterResponse> DeleteAsync(Guid id);
    }
}
