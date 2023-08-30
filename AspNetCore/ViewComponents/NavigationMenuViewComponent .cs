using Brupper.AspNetCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Brupper.AspNetCore.ViewComponents;

public class NavigationMenuViewComponent : ViewComponent
{
    private readonly IMenuAccessService dataAccessService;

    public NavigationMenuViewComponent(IMenuAccessService dataAccessService)
    {
        this.dataAccessService = dataAccessService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await dataAccessService.GetMenuItemsAsync(HttpContext.User);

        return View(items?.ToList() ?? new());
    }
}
