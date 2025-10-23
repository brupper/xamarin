using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Brupper.AspNetCore.Identity.Services.Users.IdentityConstants;

namespace Brupper.AspNetCore.Identity.Areas.Identity.Controllers;

[Authorize(Roles = Roles.SuperAdmin)]
[Area(AreaConstants.AreaName)]
public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return View(roles);
    }
    [HttpPost]
    public async Task<IActionResult> AddRole(string roleName)
    {
        if (roleName != null)
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
        }
        return RedirectToAction("Index");
    }
}