using University.Server.Domain.Models;

namespace University.Server.Domain.Repositories
{
    public interface ICourseRepository
    {
        Task AddAsync(Course course);
        Task<IEnumerable<Course>> ListAsync();
        Task<Course?> GetAsync(Guid id);
        void Update(Course course);
        void Remove(Course course);
    }
}
