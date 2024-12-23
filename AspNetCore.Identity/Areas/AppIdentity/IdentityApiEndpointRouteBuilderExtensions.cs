using Brupper.AspNetCore;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Routing;

internal static class IdentityApiEndpointRouteBuilderExtensions
{
    internal static IEndpointRouteBuilder MapInfoUrl<TUser>(this IEndpointRouteBuilder endpoints)
        where TUser : class, new()
    {
        var routeGroup = endpoints.MapGroup("");
        var accountGroup = routeGroup.MapGroup("/manage").RequireAuthorization();
        accountGroup.MapGet("/pingauth", (ClaimsPrincipal user) =>
        {
            var email = user.FindFirstValue(ClaimTypes.Email); // get the user's email from the claim
            return Results.Json(new { Email = email });
            ; // return the email as a plain text response
        }).RequireAuthorization();

        accountGroup.MapGet("/detailedinfo", async Task<Results<Ok<DetailedInfoResponse>, ValidationProblem, NotFound>>
            (ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider sp) =>
        {
            var userManager = sp.GetRequiredService<UserManager<TUser>>();
            if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(await CreateDetailedInfoResponseAsync(user, userManager, sp));
        });

        return endpoints;
    }

    private static async Task<DetailedInfoResponse> CreateDetailedInfoResponseAsync<TUser>(TUser user, UserManager<TUser> userManager, IServiceProvider sp)
        where TUser : class, new()
    {
        var tenantRepository = sp.GetRequiredService<ITenantRepository>();
        var brandContextAccessor = sp.GetRequiredService<IBrandContextAccessor>();
        var appUser = user as Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities.User;

        var tenant = await tenantRepository.GetByIdAsync(appUser.TenantId);

        return new()
        {
            Modules = tenant.Licences,
            BrandId = (await brandContextAccessor.GetBrandIdAsync()),
            TenantId = appUser?.TenantId,
            Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
        };
    }

    public sealed class DetailedInfoResponse
    {
        public required List<ModuleReference> Modules { get; init; }

        public required string? BrandId { get; init; }

        public required string TenantId { get; init; }

        public required string Email { get; init; }

        public required bool IsEmailConfirmed { get; init; }
    }
}
