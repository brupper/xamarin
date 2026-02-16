using Brupper.AspNetCore.Identity.Models;
using Brupper.AspNetCore.Identity.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Brupper.AspNetCore.Identity.Services.Users.IdentityConstants;

namespace Brupper.AspNetCore.Identity.Controllers;

[Route("api/[area]/[controller]")]
[Authorize(Roles = Roles.SuperAdmin)]
public class PermissionsController(RoleManager<IdentityRole> roleManager) : BaseController
{
    [HttpGet("{roleId}")]
    public async Task<ActionResult<PermissionViewModel>> GetAsync(string roleId)
    {
        var model = new PermissionViewModel();
        var allPermissions = new List<RoleClaimsViewModel>();
        allPermissions.GetPermissions(typeof(DefaultPermissions.Products), roleId);

        var role = await roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return NotFound();
        }

        model.RoleId = roleId;
        var claims = await roleManager.GetClaimsAsync(role);
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
        return Ok(model);
    }

    [HttpPut("{roleId}")]
    public async Task<IActionResult> UpdateAsync(string roleId, [FromBody] PermissionViewModel model)
    {
        if (roleId != model.RoleId)
        {
            return BadRequest("Role ID mismatch");
        }

        var role = await roleManager.FindByIdAsync(model.RoleId);
        if (role == null)
        {
            return NotFound();
        }

        var claims = await roleManager.GetClaimsAsync(role);
        foreach (var claim in claims)
        {
            await roleManager.RemoveClaimAsync(role, claim);
        }

        var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();
        foreach (var claim in selectedClaims)
        {
            await roleManager.AddPermissionClaim(role, claim.Value);
        }

        return NoContent();
    }
}
