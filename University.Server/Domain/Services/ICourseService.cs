using University.Server.Domain.Models;

namespace University.Server.Domain.Services
{
    public interface ICourseService
    {
        Task<Course> SaveAsync(Course course);

        Task<IEnumerable<Course>> GetManyAsync(IEnumerable<Guid> ids);

        Task<Course?> GetAsyncNullable(Guid id);

        Task<Course> GetAsync(Guid id);

        Task<IEnumerable<Course>> ListAsync();

        Task<Course> UpdateAsync(Guid id, Course course);

        Task<Course> PatchStudentsAsync(Guid id, PatchModel<User> patch);

        Task<Course> PatchModulesAsync(Guid id, PatchModel<Module> patch);

        Task DeleteAsync(Guid id);
    }
}