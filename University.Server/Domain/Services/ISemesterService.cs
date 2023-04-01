﻿using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface ISemesterService
    {
        Task<Response<Semester>> SaveAsync(Semester semester);
        Task<Response<Semester>> CalculateAsync(Guid id);
        Task<IEnumerable<Semester>> GetManyAsync(ICollection<Guid> ids);
        Task<IEnumerable<Semester>> GetManyAsyncByTime(DateTime containsDate, TimeSpan? delta = null);
        Task<Semester?> GetAsyncNullable(Guid id);
        Task<Response<Semester>> GetAsync(Guid id);
        Task<IEnumerable<Semester>> ListAsync();
        Task<Response<Semester>> PatchModulesAsync(Guid id, PatchModel<Module> patch);
        Task<Response<Semester>> DeleteAsync(Guid id);
        Task<IEnumerable<SemesterModule>> GetActiveSemesterModulesOfUser(Guid id);
    }
}
