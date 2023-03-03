using University.Server.Domain.Models;

namespace University.Server.Domain.Repositories
{
    public interface ILocationRepository
    {
        Task AddAsync(Location location);
        Task<IEnumerable<Location>> ListAsync();
        Task<Location?> GetAsync(Guid id);
        void Remove(Location location);
    }
}
