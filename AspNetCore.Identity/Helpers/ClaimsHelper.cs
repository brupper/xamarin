using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Security.Claims;

namespace Brupper.AspNetCore.Identity;

public static class ClaimsHelper
{
    public const string PermissionClaimType = "Permissions";

    public static void GetPermissions(this List<RoleClaimsViewModel> allPermissions, Type policy, string roleId)
    {
        var fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

        foreach (var fi in fields)
        {
            allPermissions.Add(new RoleClaimsViewModel { Value = fi?.GetValue(null)?.ToString() ?? string.Empty, Type = PermissionClaimType });
        }
    }

    public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string permission)
    {
        var allClaims = await roleManager.GetClaimsAsync(role);
        if (!allClaims.Any(a => a.Type == PermissionClaimType && a.Value == permission))
        {
            await roleManager.AddClaimAsync(role, new Claim(PermissionClaimType, permission));
        }
    }
}

