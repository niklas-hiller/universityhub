using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using University.Server.Domain.Models;
using University.Server.Domain.Services;

namespace University.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly EAuthorization auth;

        public PermissionAttribute(EAuthorization auth)
        {
            this.auth = auth;
        }

        private bool HasAuthorization(ClaimsPrincipal user, EAuthorization authorization)
        {
            const string CLAIM = "authorization";
            switch (authorization)
            {
                case EAuthorization.Student:
                    return user.HasClaim(CLAIM, EAuthorization.Student.ToString())
                        || user.HasClaim(CLAIM, EAuthorization.Professor.ToString())
                        || user.HasClaim(CLAIM, EAuthorization.Administrator.ToString());
                case EAuthorization.Professor:
                    return user.HasClaim(CLAIM, EAuthorization.Professor.ToString())
                        || user.HasClaim(CLAIM, EAuthorization.Administrator.ToString());
                case EAuthorization.Administrator:
                    return user.HasClaim(CLAIM, EAuthorization.Administrator.ToString());
                default:
                    return false;
            }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!HasAuthorization(user, auth))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
