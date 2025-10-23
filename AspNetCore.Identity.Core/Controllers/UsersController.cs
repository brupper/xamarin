using Brupper.AspNetCore.Identity.Entities;
using Brupper.AspNetCore.Identity.Models;
using Brupper.AspNetCore.Identity.Repositories;
using Brupper.AspNetCore.Identity.Services.Communication;
using Brupper.AspNetCore.Identity.Services.Users;
using Brupper.AspNetCore.Identity.Resources;
using AutoMapper;
using Brupper.AspNetCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brupper.AspNetCore.Identity.Controllers;

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

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserEditViewModel>> GetByIdAsync(string userId)
    {
        var currentUser = await userManager.GetUserAsync(HttpContext.User);
        var isSuperAdmin = User.Identities.Any(x => x.IsSuperAdmin());

        var tenants = await GetAvailableTenantsAsync(currentUser?.TenantId);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var model = mapper.Map<UserEditViewModel>(user);
        model.Tenants = tenants.ToList();

        return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult<UserEditViewModel>> CreateAsync([FromBody] UserEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userExists = await userManager.FindByEmailAsync(model.Email);

        if (userExists != null)
        {
            return BadRequest("User with this email already exists");
        }

        await userService.CreateAsync(model);

        var createdUser = await userManager.FindByEmailAsync(model.Email);
        if (createdUser != null)
        {
            await emailSender.SendAccountConfirmEmail(model.Email, "Identity/Account/ResetPassword");
        }

        var result = mapper.Map<UserEditViewModel>(createdUser);
        return CreatedAtAction(nameof(GetByIdAsync), new { userId = createdUser?.Id }, result);
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult<UserEditViewModel>> UpdateAsync(string userId, [FromBody] UserEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (userId != model.Id)
        {
            return BadRequest("User ID mismatch");
        }

        await userService.UpdateAsync(model);

        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser != null)
        {
            await signInManager.RefreshSignInAsync(currentUser);
        }

        var updatedUser = await userManager.FindByIdAsync(userId);
        var result = mapper.Map<UserEditViewModel>(updatedUser);

        return Ok(result);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteAsync(string userId)
    {
        var currentUser = await userManager.GetUserAsync(HttpContext.User);
        if (userId == currentUser?.Id)
        {
            return BadRequest("You cannot delete your own account");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        await userService.RemoveUserAsync(userId);

        return NoContent();
    }

    [HttpPost("{userId}/reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        try
        {
            await emailSender.SendPasswordRecoveryEmail(user.Email, "Identity/Account/ResetPassword");
            return Ok(new { message = "Password reset email sent successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending password reset email");
            return StatusCode(500, new { message = "Failed to send password reset email" });
        }
    }

    [HttpPost("{userId}/confirm-email")]
    public async Task<IActionResult> SendEmailConfirmationAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        try
        {
            await emailSender.SendAccountConfirmEmail(user.Email, "Identity/Account/ResetPassword");
            return Ok(new { message = "Confirmation email sent successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending confirmation email");
            return StatusCode(500, new { message = "Failed to send confirmation email" });
        }
    }

    [HttpPost("{userId}/lock")]
    public async Task<IActionResult> LockAsync(string userId)
    {
        var currentUser = await userManager.GetUserAsync(HttpContext.User);
        if (userId == currentUser?.Id)
        {
            return BadRequest("You cannot lock your own account");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        try
        {
            await userService.LockUserAsync(userId);
            return Ok(new { message = "User locked successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error locking user");
            return StatusCode(500, new { message = "Failed to lock user" });
        }
    }

    [HttpPost("{userId}/unlock")]
    public async Task<IActionResult> UnlockAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        try
        {
            await userService.UnlockUserAsync(userId);
            return Ok(new { message = "User unlocked successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error unlocking user");
            return StatusCode(500, new { message = "Failed to unlock user" });
        }
    }

    [NonAction]
    private async Task<IEnumerable<Tenant>> GetAvailableTenantsAsync(string? tenantId)
    {
        var isSuperAdmin = User.Identities.Any(x => x.IsSuperAdmin());

        var tenants = await tenantRepository.GetAsync(t => isSuperAdmin || t.Id == tenantId);

        return [.. tenants.OrderBy(x => x.Name)];
    }
}
