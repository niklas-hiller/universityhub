using University.Server.Domain.Models;

namespace University.Server.Domain.Repositories
{
    public interface ISemesterRepository
    {
        Task AddAsync(Semester semester);
    }
}
