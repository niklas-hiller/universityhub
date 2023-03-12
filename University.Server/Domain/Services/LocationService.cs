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

        public async Task<Response<Location>> SaveAsync(Location location)
        {
            _logger.LogInformation("Attempting to save new location...");
            try
            {
                await _locationRepository.AddItemAsync(location);

                return new Response<Location>(StatusCodes.Status201Created, location);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<Location>(StatusCodes.Status400BadRequest, $"An error occurred when saving the location: {ex.Message}");
            }
        }

        public async Task<Location?> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing location...");

            return await _locationRepository.GetItemAsync(id);
        }

        public async Task<IEnumerable<Location>> ListAsync()
        {
            _logger.LogInformation("Attempting to retrieve existing locations...");

            return await _locationRepository.GetItemsAsync("SELECT * FROM c");
        }

        public async Task<Response<Location>> UpdateAsync(Guid id, Location location)
        {
            _logger.LogInformation("Attempting to update existing location...");

            var existingLocation = await _locationRepository.GetItemAsync(id);

            if (existingLocation == null)
                return new Response<Location>(StatusCodes.Status404NotFound, "Location not found.");

            existingLocation.Name = location.Name;
            existingLocation.Size = location.Size;

            try
            {
                await _locationRepository.UpdateItemAsync(existingLocation.Id, existingLocation);

                return new Response<Location>(StatusCodes.Status200OK, existingLocation);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<Location>(StatusCodes.Status400BadRequest, $"An error occurred when updating the location: {ex.Message}");
            }
        }

        public async Task<Response<Location>> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing location...");

            var existingLocation = await _locationRepository.GetItemAsync(id);

            if (existingLocation == null)
                return new Response<Location>(StatusCodes.Status404NotFound, "Location not found.");

            try
            {
                await _locationRepository.DeleteItemAsync(existingLocation.Id);

                return new Response<Location>(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<Location>(StatusCodes.Status400BadRequest, $"An error occurred when deleting the location: {ex.Message}");
            }
        }
    }
}
