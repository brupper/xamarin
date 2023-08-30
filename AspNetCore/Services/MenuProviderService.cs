using Brupper.AspNetCore.Models;
using System.Security.Claims;

namespace Brupper.AspNetCore.Services;

/// <inheritdoc/>
public class MenuProviderService : IMenuProviderService
{
    // private readonly ApplicationDbContext _context;
    // public MenuAccessService(ApplicationDbContext context){_context = context;}

    /// <inheritdoc/>
    public async Task<IEnumerable<NavigationMenuViewModel>> GetMenuItemsAsync(ClaimsPrincipal? principal)
    {
        await Task.CompletedTask; // placeholder

        var isAuthenticated = principal?.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated) return new NavigationMenuViewModel[0];

        /*
        var roleIds = await GetUserRoleIds(principal);
        var query = from menu in _context.RoleMenuPermission
                    where roleIds.Contains(menu.RoleId)
                    select menu;
        var data = (await query.ToListAsync())
            .Select(m => mapper.Map<NavigationMenuViewModel>(x.NavigationMenu)).Distinct();
        */

        var data = new NavigationMenuViewModel[] {
            // new() { Id = Guid.Empty.ToString(), ControllerName = "", ActionName = "index", Name = "Default home" },
        };

        return data;
    }

    //private Task<IEnumerable<string>> GetUserRoleIds(ClaimsPrincipal ctx)
    //{
    //    var userId = GetUserId(ctx);
    //    return (from role in _context.UserRoles where role.UserId == userId select role.RoleId).ToListAsync();
    //}

    // private string? GetUserId(ClaimsPrincipal user) => (user?.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
