using University.Server.Domain.Models;

namespace University.Server.Domain.Services
{
    public interface IModuleService
    {
        Task<Module> SaveAsync(Module module);

        Task<IEnumerable<Module>> GetManyAsync(IEnumerable<Guid> ids);

        Task<Module> GetAsync(Guid id, bool excludeArchived = true);

        Task<IEnumerable<Module>> ListAsync(EModuleType? moduleType);

        Task<Module> UpdateAsync(Guid id, Module module);

        Task<Module> PatchProfessorsAsync(Guid id, PatchModel<User> patch);

        Task DeleteAsync(Guid id);
    }
}