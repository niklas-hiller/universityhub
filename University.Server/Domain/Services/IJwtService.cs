using System.Security.Claims;
using University.Server.Domain.Models;

namespace University.Server.Domain.Services
{
    public interface IJwtService
    {
        Task<Token> LoginAsync(string email, string password);

        bool IsSelf(ClaimsPrincipal user, User expectedUser);
    }
}