using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface ILocationService
    {
        Task<LocationResponse> SaveAsync(Location location);
        Task<Location?> GetAsync(Guid id);
        Task<IEnumerable<Location>> ListAsync();
        Task<LocationResponse> UpdateAsync(Guid id, Location location);
        Task<LocationResponse> DeleteAsync(Guid id);
    }
}
