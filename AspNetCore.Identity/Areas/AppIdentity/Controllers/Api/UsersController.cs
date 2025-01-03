using Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Repositories;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Communication;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users;
using Brupper.AspNetCore.Identity.Resources;
using AutoMapper;
using Brupper.AspNetCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Controllers.Api;

[Route("api/[area]/[controller]")]
public class UsersController(
    IMapper mapper,
    ITenantRepository tenantRepository,
    IUserService userService,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IdentityEmailService<User> emailSender,
    ILogger<UsersController> logger)
    : BaseController
{
    [HttpGet]
    public async Task<ActionResult<UserListViewModel>> GetAsync()
    {
        var currentUser = await userManager.GetUserAsync(HttpContext.User);
        var isSuperAdmin = User.Identities.Any(x => x.IsSuperAdmin());

        var tenants = await GetAvailableTenantsAsync(currentUser?.TenantId);

        var allUsersExceptCurrentUser = (await userManager.Users.ToListAsync()).AsQueryable();

        if (!isSuperAdmin)
        {
            allUsersExceptCurrentUser = allUsersExceptCurrentUser
                .Where(x => x.TenantId == currentUser!.TenantId)
                .AsQueryable();
        }

        var userModels = mapper.ProjectTo<UserEditViewModel>(allUsersExceptCurrentUser).ToList();
        userModels.ForEach(model => model.Tenants = tenants.ToList());

        userModels = userModels.OrderBy(user => tenants.FirstOrDefault(x => x.Id == user.TenantId)?.Name).ToList();


        return Ok(new UserListViewModel
        {
            Items = userModels,
            Tenants = tenants.ToList(),
        });
    }


    [NonAction]
    private async Task<IEnumerable<Tenant>> GetAvailableTenantsAsync(string? tenantId)
    {
        var isSuperAdmin = User.Identities.Any(x => x.IsSuperAdmin());

        var tenants = await tenantRepository.GetAsync(t => isSuperAdmin || t.Id == tenantId);

        return [.. tenants.OrderBy(x => x.Name)];
    }
}
