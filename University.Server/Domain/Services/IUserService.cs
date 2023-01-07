using University.Server.Domain.Models;

namespace University.Server.Domain.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> ListAsync();
        Task<User?> GetAsync(Guid id);
    }
}
