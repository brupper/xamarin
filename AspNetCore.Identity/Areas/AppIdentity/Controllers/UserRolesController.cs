using Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Controllers;

[Authorize(Services.Users.IdentityConstants.Roles.SuperAdmin)]
public class UserRolesController : AAreaController
{
    #region Fields

    private readonly SignInManager<User> signInManager;
    private readonly IUserStore<User> userStore;
    private readonly UserManager<User> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    #endregion

    #region Constructor

    public UserRolesController(
        IUserStore<User> userStore,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<HomeController> logger)
        : base(logger)
    {
        this.userStore = userStore;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
    }

    #endregion

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(string userId)
    {
        var viewModel = new List<UserRolesViewModel>();
        var user = await userManager.FindByIdAsync(userId);

        foreach (var role in roleManager.Roles.ToList())
        {
            var userRolesViewModel = new UserRolesViewModel
            {
                RoleName = role.Name
            };

            if (await userManager.IsInRoleAsync(user, role.Name))
            {
                userRolesViewModel.Selected = true;
            }
            else
            {
                userRolesViewModel.Selected = false;
            }

            viewModel.Add(userRolesViewModel);
        }

        var model = new ManageUserRolesViewModel
        {
            UserId = userId,
            UserRoles = viewModel
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, ManageUserRolesViewModel model, bool continueEditing = false)
    {
        var user = await userManager.FindByIdAsync(id);
        var roles = await userManager.GetRolesAsync(user);
        var result = await userManager.RemoveFromRolesAsync(user, roles);
        result = await userManager.AddToRolesAsync(user, model.UserRoles.Where(x => x.Selected).Select(y => y.RoleName));

        var currentUser = await userManager.GetUserAsync(User);
        await signInManager.RefreshSignInAsync(currentUser);

        if (!continueEditing)
        {
            return RedirectToAction(nameof(Edit), "Users");
        }

        return RedirectToAction(nameof(Edit), new { userId = id });
    }

    #endregion
}
