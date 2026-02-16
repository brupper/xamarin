using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brupper.AspNetCore.Identity.Controllers;

public static class AreaApiConstants
{
    public const string AreaName = "identity";
}

[ApiController]
[Authorize]
// [Authorize(Policy = IdentityConstants.AuthorizationPolicy)]
[Area(AreaApiConstants.AreaName)]
public abstract class BaseController : ControllerBase
{
    public virtual string Email => User.GetEmail();
    public virtual string UserId => User.GetUserId();
}
