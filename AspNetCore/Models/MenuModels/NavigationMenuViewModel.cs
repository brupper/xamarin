using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Brupper.AspNetCore.Models;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class NavigationMenuViewModel
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? ParentMenuId { get; set; }

    public NavigationMenuViewModel? ParentNavigationMenu { get; set; }

    public string? AreaName { get; set; }

    public string ControllerName { get; set; } = default!;

    public string ActionName { get; set; } = default!;

    [NotMapped]
    public bool Permitted { get; set; }
}
