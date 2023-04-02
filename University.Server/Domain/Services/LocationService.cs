using Microsoft.IdentityModel.Tokens;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Exceptions;

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

        public async Task<Location> SaveAsync(Location location)
        {
            _logger.LogInformation("Attempting to save new location...");

            try
            {
                await _locationRepository.AddItemAsync(location);

                return location;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when saving the location: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Location>> GetManyAsync(IEnumerable<Guid> ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return Enumerable.Empty<Location>();
            }
            var query = $"SELECT * FROM c WHERE c.id IN ('{string.Join("', '", ids)}')";
            try
            {
                return await _locationRepository.GetItemsAsync(query);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                _logger.LogInformation($"Cosmos DB Exception for: {query})");
                throw ex;
            }
        }

        public async Task<Location?> GetAsyncNullable(Guid id)
        {
            try
            {
                var location = await _locationRepository.GetItemAsync(id);

                return location;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<Location> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing location...");

            try
            {
                var location = await _locationRepository.GetItemAsync(id);

                return location;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when retrieving the location: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Location>> ListAsync()
        {
            _logger.LogInformation("Attempting to retrieve existing locations...");

            return await _locationRepository.GetItemsAsync("SELECT * FROM c");
        }

        public async Task<Location> UpdateAsync(Guid id, Location location)
        {
            _logger.LogInformation("Attempting to update existing location...");

            var existingLocation = await GetAsync(id);

            existingLocation.Name = location.Name;
            existingLocation.Size = location.Size;

            try
            {
                await _locationRepository.UpdateItemAsync(existingLocation.Id, existingLocation);

                return existingLocation;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when updating the location: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing location...");

            try
            {
                await _locationRepository.DeleteItemAsync(id);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when deleting the location: {ex.Message}");
            }
        }
    }
}