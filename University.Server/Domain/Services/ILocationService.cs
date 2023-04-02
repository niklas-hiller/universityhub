using University.Server.Domain.Models;

namespace University.Server.Domain.Services
{
    public interface ILocationService
    {
        Task<Location> SaveAsync(Location location);

        Task<IEnumerable<Location>> GetManyAsync(IEnumerable<Guid> ids);

        Task<Location> GetAsync(Guid id, bool excludeArchived = true);

        Task<IEnumerable<Location>> ListAsync();

        Task<Location> UpdateAsync(Guid id, Location location);

        Task DeleteAsync(Guid id);
    }
}