using Brupper.AspNetCore.Identity.Entities;
using Brupper.AspNetCore.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Brupper.AspNetCore.Identity.Controllers;

[Route("api/[area]/[controller]")]
[Authorize(Services.Users.IdentityConstants.Roles.SuperAdmin)]
public class UserRolesController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<UserRolesController> logger)
    : BaseController
{
    [HttpGet("{userId}")]
    public async Task<ActionResult<ManageUserRolesViewModel>> GetAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var viewModel = new List<UserRolesViewModel>();

        foreach (var role in roleManager.Roles.ToList())
        {
            var userRolesViewModel = new UserRolesViewModel
            {
                RoleName = role.Name,
                Selected = await userManager.IsInRoleAsync(user, role.Name)
            };

            viewModel.Add(userRolesViewModel);
        }

        var model = new ManageUserRolesViewModel
        {
            UserId = userId,
            UserRoles = viewModel
        };

        return Ok(model);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateAsync(string userId, [FromBody] ManageUserRolesViewModel model)
    {
        if (userId != model.UserId)
        {
            return BadRequest("User ID mismatch");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await userManager.GetRolesAsync(user);
        var removeResult = await userManager.RemoveFromRolesAsync(user, roles);

        if (!removeResult.Succeeded)
        {
            return BadRequest(removeResult.Errors);
        }

        var selectedRoles = model.UserRoles.Where(x => x.Selected).Select(y => y.RoleName);
        var addResult = await userManager.AddToRolesAsync(user, selectedRoles);

        if (!addResult.Succeeded)
        {
            return BadRequest(addResult.Errors);
        }

        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser != null)
        {
            await signInManager.RefreshSignInAsync(currentUser);
        }

        return NoContent();
    }
}
