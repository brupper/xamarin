using System.Security.Claims;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users;

public static partial class IdentityConstants
{
    public const string ModuleClaimType = "moduleid";
    public const string TenantClaimType = "tid";
    public const string DisplayNameClaimType = "usrdname";
    
    public const string AuthorizationPolicy = "RequireAdministratorRole";
    public const string UserAdminRolePolicy = "User Administrators";

    public static class Roles
    {
        public const string SuperAdmin = nameof(DefaultRoles.SuperAdmin);
        public const string TenantAdmin = nameof(DefaultRoles.TenantAdmin);
        public const string RegularUser = nameof(DefaultRoles.Basic);

        public static IReadOnlyList<string> AllRoleNames { get; } = new List<string>
        {
            SuperAdmin,
            TenantAdmin,
            RegularUser
        }.AsReadOnly();
    }
    
    public static class Claims
    {
        public const string Role = "Role";
        public const string Permission = "Permission";
    }

    public static bool IsSuperAdmin(this ClaimsIdentity identity) => identity.HasClaim(Claims.Role, Roles.SuperAdmin);
}
