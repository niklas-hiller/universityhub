using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface ICourseService
    {
        Task<Response<Course>> SaveAsync(Course course);
        Task<Course?> GetAsync(Guid id);
        Task<IEnumerable<Course>> ListAsync();
        Task<Response<Course>> UpdateAsync(Guid id, Course course);
        Task<Response<Course>> DeleteAsync(Guid id);
    }
}
