using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> ListAsync();
        Task<User?> GetAsync(Guid id);
        Task<SaveUserResponse> SaveAsync(User user);
    }
}
