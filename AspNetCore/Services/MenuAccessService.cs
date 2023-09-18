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
        var items = await Task.WhenAll(dataAccessServices.Select(x => x.GetMenuItemsAsync(principal)));

        // TODO: order
        return items?.SelectMany(x => x)?.ToList() ?? new();
    }
}
