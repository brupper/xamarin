using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brupper.AspNetCore.Identity.Areas.Identity.Pages.Admin;

public class DashboardModel : AdminPageModel
{

    public DashboardModel(UserManager<IdentityUser> userMgr)
        => UserManager = userMgr;

    public UserManager<IdentityUser> UserManager { get; set; }

    public int UsersCount { get; set; } = 0;
    public int UsersUnconfirmed { get; set; } = 0;
    public int UsersLockedout { get; set; } = 0;
    public int UsersTwoFactor { get; set; } = 0;

    private readonly string[] emails = {
        "alice@example.com", "bob@example.com", "charlie@example.com"
    };

    public async Task OnGet()
    {
        UsersCount = UserManager.Users.Count();
        UsersUnconfirmed = UserManager.Users
            .Where(u => !u.EmailConfirmed).Count();
        UsersLockedout = (await UserManager.Users
            .Where(u => u.LockoutEnabled).ToListAsync())
            .Where(u=> u.LockoutEnd > DateTimeOffset.Now)
            .Count();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        foreach (IdentityUser existingUser in UserManager.Users.ToList())
        {
            IdentityResult result = await UserManager.DeleteAsync(existingUser);
            result.Process(ModelState);
        }
        foreach (string email in emails)
        {
            IdentityUser userObject = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            IdentityResult result = await UserManager.CreateAsync(userObject);
            if (result.Process(ModelState))
            {
                result = await UserManager.AddPasswordAsync(userObject, "mysecret");
                result.Process(ModelState);
            }
        }
        if (ModelState.IsValid)
        {
            return RedirectToPage();
        }
        return Page();
    }
}
