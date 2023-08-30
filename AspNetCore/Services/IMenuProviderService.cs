using Brupper.AspNetCore.Models;
using System.Security.Claims;

namespace Brupper.AspNetCore.Services;

/// <summary> </summary>
public interface IMenuProviderService
{
    /// <summary> </summary>
    Task<IEnumerable<NavigationMenuViewModel>> GetMenuItemsAsync(ClaimsPrincipal? principal);
}
