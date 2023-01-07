using University.Server.Domain.Models;
using University.Server.Domain.Persistence;

namespace University.Server.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;

        public UserService(ILogger<UserService> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> ListAsync()
        {
            return await _userRepository.ListAsync();
        }

        public async Task<User?> GetAsync(Guid id)
        {
            return await _userRepository.GetAsync(id);
        }
    }
}
