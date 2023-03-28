﻿using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface ICourseService
    {
        Task<Response<Course>> SaveAsync(Course course);
        Task<IEnumerable<Course>> GetManyAsync(ICollection<Guid> ids);
        Task<Course?> GetAsyncNullable(Guid id);
        Task<Response<Course>> GetAsync(Guid id);
        Task<IEnumerable<Course>> ListAsync();
        Task<Response<Course>> UpdateAsync(Guid id, Course course);
        Task<Response<Course>> PatchStudentsAsync(Guid id, PatchModel<User> patch);
        Task<Response<Course>> PatchModulesAsync(Guid id, PatchModel<Module> patch);
        Task<Response<Course>> DeleteAsync(Guid id);
    }
}
