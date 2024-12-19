using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users.IdentityConstants;

namespace Brupper.AspNetCore.Identity.Areas.Identity.Controllers;

[Authorize(Roles = Roles.SuperAdmin)]
[Area(AreaConstants.AreaName)]
public class PermissionController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public PermissionController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<ActionResult> Index(string roleId)
    {
        var model = new PermissionViewModel();
        var allPermissions = new List<RoleClaimsViewModel>();
        allPermissions.GetPermissions(typeof(DefaultPermissions.Products), roleId);
        var role = await _roleManager.FindByIdAsync(roleId);
        model.RoleId = roleId;
        var claims = await _roleManager.GetClaimsAsync(role);
        var allClaimValues = allPermissions.Select(a => a.Value).ToList();
        var roleClaimValues = claims.Select(a => a.Value).ToList();
        var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
        foreach (var permission in allPermissions)
        {
            if (authorizedClaims.Any(a => a == permission.Value))
            {
                permission.Selected = true;
            }
        }
        model.RoleClaims = allPermissions;
        return View(model);
    }

    public async Task<IActionResult> Update(PermissionViewModel model)
    {
        var role = await _roleManager.FindByIdAsync(model.RoleId);
        var claims = await _roleManager.GetClaimsAsync(role);
        foreach (var claim in claims)
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }
        var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();
        foreach (var claim in selectedClaims)
        {
            await _roleManager.AddPermissionClaim(role, claim.Value);
        }
        return RedirectToAction("Index", new { roleId = model.RoleId });
    }
}
