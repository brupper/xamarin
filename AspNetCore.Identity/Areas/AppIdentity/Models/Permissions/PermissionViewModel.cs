namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;

public class PermissionViewModel
{
    public string RoleId { get; set; } = default!;
    public List<RoleClaimsViewModel> RoleClaims { get; set; } = new();
}

public class RoleClaimsViewModel
{
    public string Type { get; set; } = default!;
    public string Value { get; set; } = default!;
    public bool Selected { get; set; }
}
