using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface IModuleService
    {
        Task<ModuleResponse> SaveAsync(Module module);
        Task<IEnumerable<Module>> ListAsync();
        Task<Module?> GetAsync(Guid id);
        Task<ModuleResponse> UpdateAsync(Guid id, Module module);
        Task<ModuleResponse> UpdateProfessorsAsync(Module module);
    }
}
