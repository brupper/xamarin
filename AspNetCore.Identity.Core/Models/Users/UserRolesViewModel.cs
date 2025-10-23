namespace Brupper.AspNetCore.Identity.Models;

public class ManageUserRolesViewModel
{
    public string UserId { get; set; } = default!;
    public List<UserRolesViewModel> UserRoles { get; set; } = new();
}

public class UserRolesViewModel
{
    public string RoleName { get; set; } = default!;
    public bool Selected { get; set; }
}
