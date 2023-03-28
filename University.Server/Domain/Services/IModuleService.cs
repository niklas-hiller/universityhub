using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface IModuleService
    {
        Task<Response<Module>> SaveAsync(Module module);
        Task<IEnumerable<Module>> GetManyAsync(ICollection<Guid> ids);
        Task<Module?> GetAsyncNullable(Guid id, bool excludeArchived = true);
        Task<Response<Module>> GetAsync(Guid id);
        Task<IEnumerable<Module>> ListAsync(EModuleType? moduleType);
        Task<Response<Module>> UpdateAsync(Guid id, Module module);
        Task<Response<Module>> PatchProfessorsAsync(Guid id, PatchModel<User> patch);
        Task<Response<Module>> DeleteAsync(Guid id);
    }
}
