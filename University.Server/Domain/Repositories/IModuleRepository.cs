using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence
{
    public interface IModuleRepository
    {
        Task AddAsync(Module module);
    }
}
