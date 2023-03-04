using University.Server.Domain.Models;

namespace University.Server.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> ListAsync();
        Task<User?> GetAsync(Guid id);
        Task AddAsync(User user);
        void Update(User user);
        void Remove(User user);
    }
}
