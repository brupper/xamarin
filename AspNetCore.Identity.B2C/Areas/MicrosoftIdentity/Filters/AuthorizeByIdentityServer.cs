using B2C;
using Brupper.AspNetCore.Caching;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Brupper.AspNetCore.Identity.B2C.Areas.MicrosoftIdentity.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AuthorizeByIdentityServerAttribute : ActionFilterAttribute, IAsyncActionFilter
{
    public const string ObjectIdentifierType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var sp = context.HttpContext.RequestServices;
        Guard.IsNotNull(sp);

        if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {

        }
        else
        {
            if (context.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                var userId = context.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ObjectIdentifierType)?.Value;
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    await next();
                    return;
                }
            }

            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }

        var settings = sp.GetService<IB2CSettings>();
        var tokenCache = sp.GetService<ICacheService>();

        //var token = Brupper.Identity.B2C.JwtTokenExtensions.ParseToken(context.HttpContext.Request);
        var jwtToken = authorizationHeader.ToString().Replace("Bearer ", "");

        // try to get token and roles from cache
        var response = tokenCache.Get<JwtSecurityToken>(jwtToken);
        if (response == null)
        {
            response = await context.HttpContext.Request.ValidateAsync(settings);
            if (response != null) // IsValid
            {
                tokenCache.Add(jwtToken, response, DateTimeOffset.Now.AddHours(3), 1);
            }
        }

        if (response == null) // !response.Valid
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }

        var identity = (ClaimsIdentity)context.HttpContext.User.Identity;
        var userIdClaim = new Claim(IdentityExtensions.ObjectIdentifierType, response.GetUniqueId()); // used by IUserContextAccessor
        identity.AddClaims(response.Claims.Union(new[] { userIdClaim }));
        identity.AddClaims(response.Claims.Union(response.Claims.Where(x => x.Type == "role").ToList()));

        await next();
    }
}
