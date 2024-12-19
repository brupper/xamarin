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

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Controllers;

public class UsersController : AAreaController
{
    #region Fields

    private readonly IMapper mapper;
    private readonly ITenantRepository tenantRepository;
    private readonly IUserService userService;
    private readonly SignInManager<User> signInManager;
    private readonly UserManager<User> userManager;
    private readonly IdentityEmailService<User> emailSender;

    #endregion

    #region Constructor

    public UsersController(
        IMapper mapper,
        ITenantRepository tenantRepository,
        IUserService userService,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IdentityEmailService<User> emailSender,
        ILogger<HomeController> logger)
        : base(logger)
    {
        this.mapper = mapper;
        this.tenantRepository = tenantRepository;
        this.userService = userService;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.emailSender = emailSender;
    }

    #endregion

    public async Task<IActionResult> Index()
    {
        var currentUser = await userManager.GetUserAsync(HttpContext.User);
        var isSuperAdmin = User.Identities.Any(x => x.IsSuperAdmin());

        var tenants = await GetAvailableTenantsAsync(currentUser?.TenantId);

        var allUsersExceptCurrentUser = (await userManager.Users.ToListAsync()).AsQueryable();

        var userModels = mapper.ProjectTo<UserEditViewModel>(allUsersExceptCurrentUser).ToList();
        userModels.ForEach(model => model.Tenants = tenants.ToList());

        userModels = userModels.OrderBy(user => tenants.FirstOrDefault(x => x.Id == user.TenantId)?.Name).ToList();

        return View(new UserListViewModel
        {
            Items = userModels,
            Tenants = tenants.ToList(),
        });
    }

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(string userId)
    {
        if (userId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var currentUser = await userManager.GetUserAsync(HttpContext.User);
        var isSuperAdmin = User.Identities.Any(x => x.IsSuperAdmin());

        var tenants = await GetAvailableTenantsAsync(currentUser?.TenantId);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var model = mapper.Map<UserEditViewModel>(user);
        model.Tenants = tenants.ToList();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, UserEditViewModel model, bool continueEditing = false)
    {
        var userExists = await userManager.FindByEmailAsync(model.Email);

        if (id == null && userExists == null)
        {
            await userService.CreateAsync(model);

            userExists = await userManager.FindByEmailAsync(model.Email);

            await emailSender.SendConfirmationLinkAsync(userExists, model.Email, "Identity/Account/ResetPassword");

            return RedirectToAction(nameof(Index));
        }
        else
        {
            await userService.UpdateAsync(model);
        }

        var currentUser = await userManager.GetUserAsync(User);
        await signInManager.RefreshSignInAsync(currentUser);

        if (!continueEditing)
        {
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Edit), new { userId = id });
    }

    #endregion

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string userId)
    {
        if (userId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await emailSender.SendPasswordRecoveryEmail(user.Email, "Identity/Account/ResetPassword");

            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Success(Labels.general_signin_password_verificationsent));
        }
        //catch (DomainException e) when (e.Type == DomainExceptionType.OperationFailed)
        //{
        //    TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], e.ClientMessage)));
        //}
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed($"{ex}"));
            //TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], ex)));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EmailConfirm(string userId)
    {
        if (userId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await emailSender.SendConfirmationLinkAsync(user, user.Email, "Identity/Account/ResetPassword");

            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Success(Labels.general_signin_password_verificationsent));
        }
        //catch (DomainException e) when (e.Type == DomainExceptionType.OperationFailed)
        //{
        //    TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], e.ClientMessage)));
        //}
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed($"{ex}"));
            //TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], ex)));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Lock(string userId)
    {
        if (userId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (userId == currentUser?.Id)
            {
                TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(Labels.users_lock_failedself));
                return RedirectToAction(nameof(Index));
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await userService.LockUserAsync(userId);

            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Success(Labels.users_lock_success));
        }
        //catch (DomainException e) when (e.Type == DomainExceptionType.OperationFailed)
        //{
        //    TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], e.ClientMessage)));
        //}
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed($"{ex}"));
            //TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], ex)));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Unlock(string userId)
    {
        if (userId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await userService.UnlockUserAsync(userId);

            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Success(Labels.users_unlock_success));
        }
        //catch (DomainException e) when (e.Type == DomainExceptionType.OperationFailed)
        //{
        //    TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], e.ClientMessage)));
        //}
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed($"{ex}"));
            //TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], ex)));
        }

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    public async Task<IActionResult> Delete(GeneralDeleteViewModel model)
    {
        if (model?.PrimaryKey == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (model.PrimaryKey == currentUser?.Id)
            {
                TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(Labels.users_delete_failedself));
                return RedirectToAction(nameof(Index));
            }

            await userService.RemoveUserAsync(model.PrimaryKey);

            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Success(Labels.users_unlock_success));
        }
        //catch (DomainException e) when (e.Type == DomainExceptionType.OperationFailed)
        //{
        //    TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], e.ClientMessage)));
        //}
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed($"{ex}"));
            //TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], ex)));
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<IEnumerable<Tenant>> GetAvailableTenantsAsync(string? tenantId)
    {
        var isSuperAdmin = User.Identities.Any(x => x.IsSuperAdmin());

        var tenants = await tenantRepository.GetAsync(t => isSuperAdmin || t.Id == tenantId);

        return tenants;
    }
}
