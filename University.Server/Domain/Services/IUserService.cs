using University.Server.Domain.Models;

namespace University.Server.Domain.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByCredentials(string email, string password);

        Task<User> SaveAsync(User user);

        Task<IEnumerable<User>> GetManyAsync(IEnumerable<Guid> ids);

        Task<User?> GetAsyncNullable(Guid id);

        Task<User> GetAsync(Guid id);

        Task<IEnumerable<User>> ListAsync(EAuthorization? authorization);

        Task<User> UpdateAsync(Guid id, User user);

        Task<User> UpdateCredentialsAsync(Guid id, User user);

        Task<User> PatchAssignmentsAsync(Guid id, PatchModel<Module> patch);

        Task<User> UpdateAssignmentAsync(Guid userId, Guid moduleId, Assignment assignment);

        Task DeleteAsync(Guid id);
    }
}