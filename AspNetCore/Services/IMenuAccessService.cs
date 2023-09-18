using Brupper.AspNetCore.Models;
using System.Security.Claims;

namespace Brupper.AspNetCore.Services;

// based on https://www.codeproject.com/Articles/5163177/MVC-6-Dynamic-Navigation-Menu-from-Database

/// <summary> </summary>
public interface IMenuAccessService
{
    /// <summary> </summary>
    Task<IEnumerable<NavigationMenuViewModel>> GetMenuItemsAsync(ClaimsPrincipal? principal);
}
/*
[Table(name: "AspNetRoleMenuPermission")]
public class RoleMenuPermission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [ForeignKey("ApplicationRole")]
    public string RoleId { get; set; }

    [ForeignKey("NavigationMenu")]
    public Guid NavigationMenuId { get; set; }

    public NavigationMenu NavigationMenu { get; set; }
}

[Table(name: "AspNetNavigationMenu")]
public class NavigationMenu
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string Name { get; set; }

    [ForeignKey("ParentNavigationMenu")]
    public Guid? ParentMenuId { get; set; }

    public virtual NavigationMenu ParentNavigationMenu { get; set; }

    public string ControllerName { get; set; }

    public string ActionName { get; set; }

    [NotMapped]
    public bool Permitted { get; set; }
}

public class MenuDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<MenuDbContext> options)
        : base(options) { }

    public DbSet<RoleMenuPermission> RoleMenuPermission { get; set; }

    public DbSet<NavigationMenu> NavigationMenu { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
*/
