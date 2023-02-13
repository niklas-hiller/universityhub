using University.Server.Domain.Models;
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
    }
}
