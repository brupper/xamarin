using B2C;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Brupper.AspNetCore.Identity.B2C.Controllers.Api;

public abstract class BaseApiController : ControllerBase
{
    protected readonly IB2CSettings b2cSettings;

    protected JwtSecurityToken securityToken;

    #region Constructor

    public BaseApiController(
        IB2CSettings b2cSettings)
    {
        this.b2cSettings = b2cSettings;
    }

    #endregion

    protected async Task<string> ValidateAndGetUserIdAsync()
    {
        securityToken = await Request.ValidateAsync(b2cSettings).ConfigureAwait(false);

        return UserId;
    }

    public string UserId => securityToken?.GetUniqueId();

    public string Email => securityToken?.GetEmail();
}
