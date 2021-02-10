using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.Identity.B2C.Filters
{
    public class AuthorizationHandler : IAuthorizationHandler
    {
        private readonly IUserGraphManager userGraphManager;

        public AuthorizationHandler(IUserGraphManager userGraphManager)
        {
            this.userGraphManager = userGraphManager;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            var userId = context?.User?.Claims?.FirstOrDefault(x => x.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            foreach (var requirement in context.Requirements.OfType<RolesAuthorizationRequirement>())
            {
                foreach (var role in requirement.AllowedRoles)
                {
                    //if (!context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value))
                    if ((await userGraphManager.IsUserInRoleAsync(userId, role).ConfigureAwait(false)))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }

}
