using University.Server.Domain.Models;
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

        public async Task<UserResponse> SaveAsync(User user)
        {
            try
            {
                await _userRepository.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                return new UserResponse(user);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new UserResponse($"An error occurred when saving the user: {ex.Message}");
            }
        }

        public async Task<UserResponse> UpdateAsync(Guid id, User user)
        {
            var existingUser = await _userRepository.GetAsync(id);

            if (existingUser == null)
                return new UserResponse("User not found.");

            if (!String.IsNullOrEmpty(user.FirstName))
            {
                existingUser.FirstName = user.FirstName;
            }
            if (!String.IsNullOrEmpty(user.LastName))
            {
                existingUser.LastName = user.LastName;
            }

            try
            {
                _userRepository.Update(existingUser);
                await _unitOfWork.CompleteAsync();

                return new UserResponse(existingUser);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new UserResponse($"An error occurred when updating the user: {ex.Message}");
            }
        }

        public async Task<User?> GetAsync(Guid id)
        {
            return await _userRepository.GetAsync(id);
        }

        public async Task<IEnumerable<User>> ListAsync(EAuthorization? authorization)
        {
            var users = await _userRepository.ListAsync();
            if (authorization != null)
            {
                users = users.Where(user => user.Authorization == authorization);
            }

            return users;
        }

        public async Task<UserResponse> DeleteAsync(Guid id)
        {
            var existingUser = await _userRepository.GetAsync(id);

            if (existingUser == null)
                return new UserResponse("User not found.");

            try
            {
                _userRepository.Remove(existingUser);
                await _unitOfWork.CompleteAsync();

                return new UserResponse(existingUser);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new UserResponse($"An error occurred when deleting the user: {ex.Message}");
            }
        }
    }
}
