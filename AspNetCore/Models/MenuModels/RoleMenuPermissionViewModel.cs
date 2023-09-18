namespace Brupper.AspNetCore.Models;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class RoleMenuPermissionViewModel
{
    public string Id { get; set; } = default!;

    public string RoleId { get; set; } = default!;

    public string NavigationMenuId { get; set; } = default!;

    public NavigationMenuViewModel NavigationMenu { get; set; } = default!;
}
