using Microsoft.AspNetCore.Authorization;

namespace Brupper.AspNetCore.Identity.Permission;

internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{

    public PermissionAuthorizationHandler()
    {

    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context == null)
        {
            return;
        }

        if (context.User == null || !(context.User?.Identity?.IsAuthenticated ?? false))
        {
            return;
        }

        var permissionss = context.User.Claims
            .Where(x => x.Type == ClaimsHelper.PermissionClaimType 
                && x.Value == requirement.Permission
                && x.Issuer == "LOCAL AUTHORITY");
        if (permissionss.Any())
        {
            context.Succeed(requirement);
            return;
        }
    }
}