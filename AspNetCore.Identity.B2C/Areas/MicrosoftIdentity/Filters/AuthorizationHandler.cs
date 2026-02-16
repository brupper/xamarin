using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Brupper.AspNetCore.Identity.B2C.Areas.MicrosoftIdentity.Filters;

public class AuthorizationHandler(IUserGraphManager userGraphManager) : IAuthorizationHandler
{
    private readonly IUserGraphManager userGraphManager = userGraphManager;

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var objecId =
            //ClaimConstants.ObjectId
            "http://schemas.microsoft.com/identity/claims/objectidentifier"
            ;

        var userId = context?.User?.Claims?.FirstOrDefault(x => x.Type == objecId)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        foreach (var requirement in (context?.Requirements?.OfType<RolesAuthorizationRequirement>() ?? Enumerable.Empty<RolesAuthorizationRequirement>()))
        {
            foreach (var role in requirement.AllowedRoles)
            {
                //if (!context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value))
                if ((await userGraphManager.IsUserInRoleAsync(userId, role).ConfigureAwait(false)))
                {
                    context?.Succeed(requirement);
                }
            }
        }
    }
}
