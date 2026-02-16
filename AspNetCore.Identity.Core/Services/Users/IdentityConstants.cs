using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Brupper.AspNetCore.Identity.Services.Users;

public static partial class IdentityConstants
{
    public const string ModuleClaimType = "moduleid";
    public const string TenantClaimType = "tid";
    public const string DisplayNameClaimType = "usrdname";

    public const string AuthorizationPolicy = "require-administrator";
    public const string UserAdminRolePolicy = "user-administrator";

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

        public static IReadOnlyList<IdentityRole> AllRoles { get; } = new List<IdentityRole>
        {
            new(SuperAdmin) { Id = "BBBBBBBB-0000-0000-0000-000000000001" },
            new(TenantAdmin) { Id = "BBBBBBBB-0000-0000-0000-000000000002" },
            new(RegularUser) { Id = "BBBBBBBB-0000-0000-0000-000000000003" },
        }.AsReadOnly();
    }


    /// <summary>
    /// Constants for claim types.
    /// </summary>
    public static class Claims
    {
        public const string Name = "name";
        public const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string Oid = "oid";
        public const string PreferredUserName = "preferred_username";
        public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";
        public const string Tid = "tid";
        // Older scope claim
        public const string Scope = "http://schemas.microsoft.com/identity/claims/scope";
        public const string OldRole = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        // Newer scope claim
        public const string Scp = "scp";
        public const string Role = "role";
        public const string Roles = "roles";

        public const string Permission = "Permission";
    }

    public static bool IsSuperAdmin(this ClaimsIdentity identity)
        => identity.HasClaim(Claims.OldRole, Roles.SuperAdmin)
        || identity.HasClaim(Claims.Role, Roles.SuperAdmin)
        || identity.HasClaim(Claims.Roles, Roles.SuperAdmin);
}
