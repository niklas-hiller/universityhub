using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Contexts;
using University.Server.Domain.Repositories;

namespace University.Server.Domain.Persistence.Repositories
{
    public class LocationRepository : BaseRepository, ILocationRepository
    {
        private readonly ILogger<LocationRepository> _logger;

        public LocationRepository(AppDbContext context, ILogger<LocationRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task AddAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
        }
    }
}
