using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Brupper.AspNetCore.Identity.Areas.Identity.Pages.Admin;

public class PasswordsModel : AdminPageModel
{

    public PasswordsModel(UserManager<IdentityUser> usrMgr,
            IdentityEmailService emailService)
    {
        UserManager = usrMgr;
        EmailService = emailService;
    }

    public UserManager<IdentityUser> UserManager { get; set; }
    public IdentityEmailService EmailService { get; set; }

    public IdentityUser IdentityUser { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; }

    [BindProperty]
    [Required]
    public string Password { get; set; }

    [BindProperty]
    [Compare(nameof(Password))]
    public string Confirmation { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("Selectuser",
                new { Label = "Password", Callback = "Passwords" });
        }
        IdentityUser = await UserManager.FindByIdAsync(Id);
        return Page();
    }

    public async Task<IActionResult> OnPostSetPasswordAsync()
    {
        if (ModelState.IsValid)
        {
            IdentityUser = await UserManager.FindByIdAsync(Id);
            if (await UserManager.HasPasswordAsync(IdentityUser))
            {
                await UserManager.RemovePasswordAsync(IdentityUser);
            }
            IdentityResult result =
                await UserManager.AddPasswordAsync(IdentityUser, Password);
            if (result.Process(ModelState))
            {
                TempData["message"] = "Password Changed";
                return RedirectToPage();
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostUserChangeAsync()
    {
        throw new NotImplementedException();

        var user = IdentityUser = await UserManager.FindByIdAsync(Id);

        /*
        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user does not exist or is not confirmed
            return RedirectToPage("./ForgotPasswordConfirmation");
        }
        */

        await UserManager.RemovePasswordAsync(IdentityUser);

        // https://github.com/dotnet/aspnetcore/blob/1dcf7acfacf0fe154adcc23270cb0da11ff44ace/src/Identity/UI/src/Areas/Identity/Pages/V5/Account/ForgotPassword.cshtml.cs

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        await EmailService.SendPasswordRecoveryEmail(IdentityUser, "/Identity/UserPasswordRecoveryConfirm");
        TempData["message"] = "Email Sent to User";

        return RedirectToPage();
    }
}
