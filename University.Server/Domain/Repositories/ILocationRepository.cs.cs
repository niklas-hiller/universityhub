using University.Server.Domain.Models;

namespace University.Server.Domain.Repositories
{
    public interface ILocationRepository
    {
        Task AddAsync(Location location);
    }
}
