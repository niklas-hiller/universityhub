using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Repositories;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILogger<LocationService> _logger;
        private readonly ILocationRepository _locationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LocationService(ILogger<LocationService> logger, ILocationRepository locationRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _locationRepository = locationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<LocationResponse> SaveAsync(Location location)
        {
            try
            {
                await _locationRepository.AddAsync(location);
                await _unitOfWork.CompleteAsync();

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
            return await _locationRepository.ListAsync();
        }

        public async Task<Location?> GetAsync(Guid id)
        {
            return await _locationRepository.GetAsync(id);
        }

        public async Task<LocationResponse> UpdateAsync(Guid id, Location location)
        {
            var existingLocation = await _locationRepository.GetAsync(id);

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
                _locationRepository.Update(existingLocation);
                await _unitOfWork.CompleteAsync();

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
            var existingLocation = await _locationRepository.GetAsync(id);

            if (existingLocation == null)
                return new LocationResponse("Location not found.");

            try
            {
                _locationRepository.Remove(existingLocation);
                await _unitOfWork.CompleteAsync();

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
