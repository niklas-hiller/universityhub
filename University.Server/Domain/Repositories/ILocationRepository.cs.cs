using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence
{
    public interface ILocationRepository
    {
        Task AddAsync(Location location);
    }
}
