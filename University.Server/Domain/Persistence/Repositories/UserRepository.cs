using Microsoft.EntityFrameworkCore;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Contexts;


namespace University.Server.Domain.Persistence.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, ILogger<UserRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<User?> GetAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> ListAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }
    }
}
