using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface ILocationService
    {
        Task<Response<Location>> SaveAsync(Location location);
        Task<Location?> GetAsync(Guid id);
        Task<IEnumerable<Location>> ListAsync();
        Task<Response<Location>> UpdateAsync(Guid id, Location location);
        Task<Response<Location>> DeleteAsync(Guid id);
    }
}
