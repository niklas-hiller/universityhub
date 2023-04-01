using University.Server.Domain.Models;

namespace University.Server.Domain.Services
{
    public interface IModuleService
    {
        Task<Module> SaveAsync(Module module);
        Task<IEnumerable<Module>> GetManyAsync(ICollection<Guid> ids);
        Task<Module?> GetAsyncNullable(Guid id, bool excludeArchived = true);
        Task<Module> GetAsync(Guid id);
        Task<IEnumerable<Module>> ListAsync(EModuleType? moduleType);
        Task<Module> UpdateAsync(Guid id, Module module);
        Task<Module> PatchProfessorsAsync(Guid id, PatchModel<User> patch);
        Task DeleteAsync(Guid id);
    }
}
