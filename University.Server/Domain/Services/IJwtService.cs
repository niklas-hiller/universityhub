using System.Security.Claims;
using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public interface IJwtService
    {
        Task<Response<Token>> LoginAsync(string email, string password);
        bool HasAuthorization(ClaimsPrincipal user, EAuthorization authorization);
        bool IsSelf(ClaimsPrincipal user, User expectedUser);
    }
}
