using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Controllers;

public class HomeController : AAreaController
{
    #region Constructor

    public HomeController(ILogger<HomeController> logger) : base(logger) { }

    #endregion

    public async Task<IActionResult> Index() => RedirectToAction(nameof(Index), new { controller = "users", area = AreaConstants.AreaName });
}
