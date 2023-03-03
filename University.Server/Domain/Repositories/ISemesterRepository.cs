using University.Server.Domain.Models;

namespace University.Server.Domain.Repositories
{
    public interface ISemesterRepository
    {
        Task AddAsync(Semester semester);
        Task<IEnumerable<Semester>> ListAsync();
        Task<Semester?> GetAsync(Guid id);
        void Remove(Semester semester);
    }
}
