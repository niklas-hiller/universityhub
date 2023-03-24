using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByCredentials(string email, string password);
        Task<Response<User>> SaveAsync(User user);
        Task<User?> GetAsync(Guid id);
        Task<IEnumerable<User>> ListAsync(EAuthorization? authorization);
        Task<Response<User>> UpdateAsync(Guid id, User user);
        Task<Response<User>> PatchAssignmentsAsync(Guid id, PatchModel<Module> patch);
        Task<Response<User>> DeleteAsync(Guid id);
    }
}
