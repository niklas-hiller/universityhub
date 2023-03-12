using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ICosmosDbRepository<User, UserEntity> _userRepository;

        public UserService(ILogger<UserService> logger, ICosmosDbRepository<User, UserEntity> userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<Response<User>> SaveAsync(User user)
        {
            _logger.LogInformation("Attempting to save new user...");

            try
            {
                await _userRepository.AddItemAsync(user);

                return new Response<User>(StatusCodes.Status201Created, user);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<User>(StatusCodes.Status400BadRequest, $"An error occurred when saving the user: {ex.Message}");
            }
        }

        public async Task<User?> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing user...");

            return await _userRepository.GetItemAsync(id);
        }

        public async Task<IEnumerable<User>> ListAsync(EAuthorization? authorization)
        {
            _logger.LogInformation("Attempting to retrieve existing users...");

            if (authorization != null)
            {
                return await _userRepository.GetItemsAsync($"SELECT * FROM c WHERE c.Authorization = '{authorization}'");
            }
            else
            {
                return await _userRepository.GetItemsAsync($"SELECT * FROM c");
            }
        }

        public async Task<Response<User>> UpdateAsync(Guid id, User user)
        {
            _logger.LogInformation("Attempting to update existing user...");

            var existingUser = await _userRepository.GetItemAsync(id);

            if (existingUser == null)
                return new Response<User>(StatusCodes.Status404NotFound, "User not found.");

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;

            try
            {
                await _userRepository.UpdateItemAsync(existingUser.Id, existingUser);

                return new Response<User>(StatusCodes.Status200OK, existingUser);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<User>(StatusCodes.Status400BadRequest, $"An error occurred when updating the user: {ex.Message}");
            }
        }

        public async Task<Response<User>> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing user...");

            var existingUser = await _userRepository.GetItemAsync(id);

            if (existingUser == null)
                return new Response<User>(StatusCodes.Status404NotFound, "User not found.");

            try
            {
                await _userRepository.DeleteItemAsync(id);

                return new Response<User>(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<User>(StatusCodes.Status400BadRequest, $"An error occurred when deleting the user: {ex.Message}");
            }
        }
    }
}
