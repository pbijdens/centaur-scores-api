using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using CentaurScores.Services;
using System.Security.Claims;

namespace CentaurScores.Attributes
{
    /// <summary>
    /// Apply this attribute to a controller method or class to require a logged-in user to access
    /// that method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        /// <see cref="IAuthorizationFilter.OnAuthorization(AuthorizationFilterContext)"></see>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            ClaimsPrincipal user = context.HttpContext.User;
            // Need user with an ID claim.
            if (!(user?.Identity?.IsAuthenticated ?? false) || !(user?.Claims?.Any(x => x.Type == TokenService.IdClaim) ?? false))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
