using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Filters;

public class AuthorizationHandler : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context == null)
        {
            return;
        }

        if (context.User == null || !(context.User?.Identity?.IsAuthenticated ?? false))
        {
            context.Fail();
            return;
        }

        // "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        // vs
        // "http://schemas.microsoft.com/identity/claims/objectidentifier" // Microsoft.Identity.Web.ClaimConstants.ObjectId
        //var userId = context.User.GetUserId();

        foreach (var requirement in (context?.Requirements?.OfType<RolesAuthorizationRequirement>() ?? Enumerable.Empty<RolesAuthorizationRequirement>()))
        {
            foreach (var role in requirement.AllowedRoles)
            {
                // SuperAdmin kell a gyari Identity feluletekhez
                if (context.User.IsInRole(role)) //.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role))
                {
                    context?.Succeed(requirement);
                }
            }
        }

        await Task.CompletedTask; // placeholder
    }
}
// */
