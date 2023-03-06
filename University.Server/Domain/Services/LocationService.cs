using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILogger<LocationService> _logger;
        private readonly ICosmosDbRepository<Location, LocationEntity> _locationRepository;

        public LocationService(ILogger<LocationService> logger, ICosmosDbRepository<Location, LocationEntity> locationRepository)
        {
            _logger = logger;
            _locationRepository = locationRepository;
        }

        public async Task<LocationResponse> SaveAsync(Location location)
        {
            try
            {
                await _locationRepository.AddItemAsync(location);

                return new LocationResponse(location);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new LocationResponse($"An error occurred when saving the location: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Location>> ListAsync()
        {
            return await _locationRepository.GetItemsAsync("SELECT * FROM c");
        }

        public async Task<Location?> GetAsync(Guid id)
        {
            return await _locationRepository.GetItemAsync(id);
        }

        public async Task<LocationResponse> UpdateAsync(Guid id, Location location)
        {
            var existingLocation = await _locationRepository.GetItemAsync(id);

            if (existingLocation == null)
                return new LocationResponse("Location not found.");

            if (!String.IsNullOrEmpty(location.Name))
            {
                existingLocation.Name = location.Name;
            }
            if (location.Seats > 0)
            {
                existingLocation.Seats = location.Seats;
            }

            try
            {
                await _locationRepository.UpdateItemAsync(existingLocation.Id, existingLocation);

                return new LocationResponse(existingLocation);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new LocationResponse($"An error occurred when updating the location: {ex.Message}");
            }
        }

        public async Task<LocationResponse> DeleteAsync(Guid id)
        {
            var existingLocation = await _locationRepository.GetItemAsync(id);

            if (existingLocation == null)
                return new LocationResponse("Location not found.");

            try
            {
                await _locationRepository.DeleteItemAsync(existingLocation.Id);

                return new LocationResponse(existingLocation);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new LocationResponse($"An error occurred when deleting the location: {ex.Message}");
            }
        }
    }
}
