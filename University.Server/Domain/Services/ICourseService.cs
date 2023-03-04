using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface ICourseService
    {
        Task<CourseResponse> SaveAsync(Course course);
        Task<IEnumerable<Course>> ListAsync();
        Task<Course?> GetAsync(Guid id);
        Task<CourseResponse> UpdateAsync(Guid id, Course course);
        Task<CourseResponse> OverwriteAsync(Guid id, Course course);
        Task<CourseResponse> DeleteAsync(Guid id);
    }
}
