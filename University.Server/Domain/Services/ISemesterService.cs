using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface ISemesterService
    {
        Task<Response<Semester>> SaveAsync(Semester semester);
        Task<Semester?> GetAsync(Guid id);
        Task<IEnumerable<Semester>> ListAsync();
        Task<Response<Semester>> DeleteAsync(Guid id);
    }
}
