using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> ListAsync();
        Task<User?> GetAsync(Guid id);
        Task AddAsync(User user);
        void Remove(User user);
    }
}
