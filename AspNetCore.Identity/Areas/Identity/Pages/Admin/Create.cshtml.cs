using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Brupper.AspNetCore.Identity.Areas.Identity.Pages.Admin;

public class CreateModel : AdminPageModel
{

    public CreateModel(UserManager<IdentityUser> mgr,
        IdentityEmailService emailService)
    {
        UserManager = mgr;
        EmailService = emailService;
    }

    public UserManager<IdentityUser> UserManager { get; set; }
    public IdentityEmailService EmailService { get; set; }

    [BindProperty(SupportsGet = true)]
    [EmailAddress]
    public string Email { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        throw new NotImplementedException();
        if (ModelState.IsValid)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = Email,
                Email = Email,
                EmailConfirmed = true
            };
            IdentityResult result = await UserManager.CreateAsync(user);
            if (result.Process(ModelState))
            {
                // https://github.com/dotnet/aspnetcore/blob/1dcf7acfacf0fe154adcc23270cb0da11ff44ace/src/Identity/UI/src/Areas/Identity/Pages/V5/Account/ForgotPassword.cshtml.cs

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713

                await EmailService.SendPasswordRecoveryEmail(user,
                    "/Identity/UserAccountComplete");
                TempData["message"] = "Account Created";
                return RedirectToPage();
            }
        }
        return Page();
    }
}
