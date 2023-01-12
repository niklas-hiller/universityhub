using University.Server.Domain.Models;

namespace University.Server.Domain.Repositories
{
    public interface IModuleRepository
    {
        Task AddAsync(Module module);
    }
}
