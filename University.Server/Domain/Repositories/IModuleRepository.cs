using University.Server.Domain.Models;

namespace University.Server.Domain.Repositories
{
    public interface IModuleRepository
    {
        Task AddAsync(Module module);
        Task<IEnumerable<Module>> ListAsync();
        Task<Module?> GetAsync(Guid id);
        void Update(Module module);
    }
}
