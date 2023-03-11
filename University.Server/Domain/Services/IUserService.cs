using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface IUserService
    {
        Task<UserResponse> SaveAsync(User user);
        Task<User?> GetAsync(Guid id);
        Task<IEnumerable<User>> ListAsync(EAuthorization? authorization);
        Task<UserResponse> UpdateAsync(Guid id, User user);
        Task<UserResponse> DeleteAsync(Guid id);
    }
}
