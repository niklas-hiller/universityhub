using Microsoft.EntityFrameworkCore;
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

        public async Task<Location?> GetAsync(Guid id)
        {
            return await _context.Locations.FindAsync(id);
        }

        public async Task<IEnumerable<Location>> ListAsync()
        {
            return await _context.Locations.ToListAsync();
        }

        public void Update(Location location)
        {
            _context.Locations.Update(location);
        }

        public void Remove(Location location)
        {
            _context.Locations.Remove(location);
        }
    }
}
