using University.Server.Domain.Models;
using University.Server.Domain.Persistence;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(ILogger<UserService> logger, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<User>> ListAsync()
        {
            return await _userRepository.ListAsync();
        }

        public async Task<User?> GetAsync(Guid id)
        {
            return await _userRepository.GetAsync(id);
        }

        public async Task<SaveUserResponse> SaveAsync(User user)
        {
            try
            {
                await _userRepository.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                return new SaveUserResponse(user);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new SaveUserResponse($"An error occurred when saving the user: {ex.Message}");
            }
        }
    }
}
