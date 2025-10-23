using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using Brupper.AspNetCore.Identity.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Controllers;

[Authorize(Policy = IdentityConstants.AuthorizationPolicy)]
[Area(AreaConstants.AreaName)]
public abstract class AAreaController : Controller
{
    protected readonly ILogger logger;

    #region Constructor

    protected AAreaController(
        ILogger logger)
    {
        this.logger = logger;
    }

    #endregion
}
