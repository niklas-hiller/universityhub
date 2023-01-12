using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface ILocationService
    {
        Task<LocationResponse> SaveAsync(Location location);
    }
}
