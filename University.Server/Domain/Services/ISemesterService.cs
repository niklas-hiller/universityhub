using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface ISemesterService
    {
        Task<SemesterResponse> SaveAsync(Semester semester);
    }
}
