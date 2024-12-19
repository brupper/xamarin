using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Filters;

public class MobileClientAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<MobileAccessTokenRequirement>//, IAuthorizationHandler
{
    #region Fields

    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    #endregion

    // https://github.com/microsoftgraph/msgraph-sdk-dotnet-core/blob/dev/src/Microsoft.Graph.Core/Extensions/ITokenValidableExtension.cs
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MobileAccessTokenRequirement requirement)
    {
        var sp = httpContextAccessor?.HttpContext?.RequestServices;
        Guard.IsNotNull(sp);

        if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Succeed(requirement);
            return;
        }

        if (!httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            context.Fail();
            return;
        }

        /*
        var settings = sp.GetService<IB2CSettings>();
        var tokenCache = sp.GetService<ICacheService>();

        //var token = Brupper.Identity.B2C.JwtTokenExtensions.ParseToken(context.HttpContext.Request);
        var jwtToken = authorizationHeader.ToString().Replace("Bearer ", "");

        // try to get token and roles from cache
        var response = tokenCache.Get<JwtSecurityToken>(jwtToken);
        if (response == null)
        {
            response = await httpContextAccessor.HttpContext.Request.ValidateAsync(settings);
            if (response != null) // IsValid
            {
                tokenCache.Add(jwtToken, response, DateTimeOffset.Now.AddHours(3), 1);
            }
        }

        if (response == null) // !response.Valid
        {
            return;
        }

        var identity = (ClaimsIdentity)httpContextAccessor.HttpContext.User.Identity;
        var userIdClaim = new Claim( /*IdentityExtensions. TODO brupper update */
        /*ObjectIdentifierType, response.GetUniqueId()); // used by IUserContextAccessor
        identity.AddClaims(response.Claims.Union(new List<Claim> { userIdClaim }));

        context.Succeed(requirement);
        */
    }

    public const string ObjectIdentifierType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
}

public class MobileAccessTokenRequirement : IAuthorizationRequirement
{
    public MobileAccessTokenRequirement() { }
}
