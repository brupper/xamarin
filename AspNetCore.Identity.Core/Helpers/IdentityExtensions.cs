using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using static Brupper.AspNetCore.Identity.Services.Users.IdentityConstants;

// https://learn.microsoft.com/en-us/dotnet/api/microsoft.identity.web.claimconstants?view=msal-model-dotnet-latest

public static class IdentityExtensions
{
    // public const string TenantType = "http://schemas.microsoft.com/identity/claims/tenantid";
    public const string ObjectIdentifierType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    public static string GetName(this ClaimsPrincipal user) =>
        user?.Claims?.FirstOrDefault(x => x.Type == DisplayNameClaimType)?.Value
        ??
        user?.Claims?.FirstOrDefault(x => x.Type == "name")?.Value ?? user?.GetEmail() ?? "ANONYMUS";

    public static string GetEmail(this ClaimsPrincipal user) => user?.Identity?.Name ?? user?.Claims?.FirstOrDefault(x => x.Type == "emails")?.Value ?? "unknown";

    public static string GetUserId(this ClaimsPrincipal user) => user?.Claims?.FirstOrDefault(x => x.Type == ObjectIdentifierType)?.Value ?? "unknown";

    public static string? GetTenant(this ClaimsPrincipal user) => user?.Claims?.FirstOrDefault(x => x.Type == TenantClaimType)?.Value;

    public static bool HasLicence(this ClaimsPrincipal user, string moduleName) => user?.Claims?.Where(x => x.Type == ModuleClaimType)?.Any(x => x.Value == moduleName) ?? false;

    public static async Task<string?> GetUserIdAsync(this AuthenticationStateProvider authenticationStateProvider)
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.FindFirst("sub")?.Value;
    }

    public static async Task<string?> GetUserNameAsync(this AuthenticationStateProvider authenticationStateProvider)
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.FindFirst("name")?.Value;
    }
}
