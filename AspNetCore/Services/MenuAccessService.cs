using Brupper.AspNetCore.Models;
using System.Security.Claims;

namespace Brupper.AspNetCore.Services;

public class MenuAccessService : IMenuAccessService
{
    private readonly IEnumerable<IMenuProviderService> dataAccessServices;

    public MenuAccessService(IEnumerable<IMenuProviderService> dataAccessServices)
    {
        this.dataAccessServices = dataAccessServices;
    }

    public async Task<IEnumerable<NavigationMenuViewModel>> GetMenuItemsAsync(ClaimsPrincipal? principal)
    {
        var items = await Task.WhenAll(dataAccessServices.Select(x => x.GetMenuItemsAsync(principal))) ?? new IEnumerable<NavigationMenuViewModel>[0];

        return items.SelectMany(x => x).OrderBy(x=>x.ParentMenuId).ThenBy(x=>x.Id).ToList();
    }
}
