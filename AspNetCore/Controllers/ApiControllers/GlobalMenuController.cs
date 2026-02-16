using Brupper.AspNetCore.Models;
using Brupper.AspNetCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Brupper.AspNetCore.Controllers;

[Authorize]
[ApiController]
[Area(BrupperAspNetCoreModule.AreaName)]
[Route("api/[area]/[controller]")]
public sealed class GlobalMenuController(
    IUserContextAccessor userContextAccessor,
    IMenuAccessService menuAccessService,
    ILogger<GlobalMenuController> logger
    ) : ControllerBase
{
    [HttpGet]
    [ActionName("GetAsync")]
    [Produces("application/json")]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
    public async Task<ActionResult<NavigationMenuViewModel[]>> GetAsync()
    {
        var areaRelatedMenu = await menuAccessService.GetMenuItemsAsync(HttpContext.User);

        return areaRelatedMenu?.ToArray() ?? [];
    }
}

public static class BrupperAspNetCoreModule
{
    public const string AreaName = "core-utils";

    public static ControllerActionEndpointConventionBuilder MapBrupperAspNetCoreModuleArea(this IEndpointRouteBuilder endpoints)
        => endpoints.MapControllerRoute(
            name: AreaName,
            pattern: "{area:exists}/{controller=home}/{action=index}/{id?}");
}