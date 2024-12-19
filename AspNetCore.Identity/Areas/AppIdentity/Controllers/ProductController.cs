using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Brupper.AspNetCore.Identity.Areas.Identity.Controllers;

[Area(AreaConstants.AreaName)]
public class ProductController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}