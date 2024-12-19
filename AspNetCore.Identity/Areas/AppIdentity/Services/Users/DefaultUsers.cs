using Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using static Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users.IdentityConstants;
using IdentityUser = Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities.User;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users;

public static class DefaultUsers
{
    #region Default data

    internal static List<Tenant> DefaultTenants { get; } = new() {
        new()
        {
            Id = "00000000-0000-0000-0000-000000000001",
            PartitionKey = "seed",
            Contact = "Baráth Ádám",
            Name = "Brupper Gp.",
            Email = "adambarath@msn.com",
            Phone = "+36706064544",
            Zip = "5143",
            City = "Jánoshida",
            Address = "Petőfi Sándor utca",
            Number = "3.",
            PostalZip = "5143",
            PostalCity = "Jánoshida",
            PostalAddress = "Petőfi Sándor utca",
            PostalNumber = "3.",
        },
        /*
        new()
        {
            Id = "00000000-0000-0000-0000-000000000002",
            PartitionKey = "seed",
            Contact = "",
            Name = "",
            Email = "",
            Phone = "",
            Zip = "",
            City = "",
            Address = "",
            Number = "",
            PostalZip = "",
            PostalCity = "",
            PostalAddress = "",
            PostalNumber = "",
        },
        // */
        /*    
        new()
        {
            Id = "00000000-0000-0000-0000-000000000003",
            PartitionKey = "seed",
            Contact = "Baráth Ádám",
            Name = "Info-Frame Kft.",
            Email = "barath.adam@infoframe.hu",
            Phone = "",
            Zip = "",
            City = "",
            Address = "",
            Number = "",
            PostalZip = "",
            PostalCity = "",
            PostalAddress = "",
            PostalNumber = "",
        },
        // */
        };

    #endregion

    public static async Task InitAndSeedDatabaseAsync(this IServiceProvider services)
    {
        using var tenantDb = services.GetService<Contexts.TenantDataContext>()!;
        await tenantDb.Database.EnsureCreatedAsync();

        using var db = services.GetService<Contexts.IdentityDataContext>()!;
        await db.Database.EnsureCreatedAsync();

        var userService = services.GetService<IUserService>()!;
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>()!;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>()!;

        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("app");

        try
        {
            //await SeedTenantsAsync(services, userManager, roleManager);
            //await SeedRolesAsync(userManager, roleManager);
            //await SeedBasicUserAsync(userService, userManager, roleManager);
            await SeedSuperAdminAsync(userService, userManager, roleManager);

            logger.LogInformation("Finished Seeding Default Data");
            logger.LogInformation("Application Starting");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "An error occurred seeding the DB");
        }
    }

    public static async Task SeedTenantsAsync(IServiceProvider services, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        using var tenantDb = services.GetService<Contexts.TenantDataContext>();
        await tenantDb.Database.EnsureCreatedAsync();

        var entities = new HashSet<string>(await tenantDb.Tenants.Select(x => x.Id).ToListAsync(), StringComparer.OrdinalIgnoreCase);
        var toInsert = DefaultTenants.Where(r => !entities.Contains(r.Id)).ToList();
        if (toInsert.Any())
        {
            tenantDb.Tenants.AddRange(toInsert);
            await tenantDb.SaveChangesAsync();
        }
    }

    public static async Task SeedRolesAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var roles = new HashSet<string>(await roleManager.Roles.Select(x => x.Name).ToListAsync(), StringComparer.OrdinalIgnoreCase);
        var rolesToInsert = Roles.AllRoleNames.Where(r => !roles.Contains(r)).ToList();

        foreach (var roleName in rolesToInsert)
        {
            var identityRole = new IdentityRole(roleName);
            await roleManager.CreateAsync(identityRole);
            await roleManager.AddClaimAsync(identityRole, new Claim(Claims.Role, roleName));
        }
    }

    public static async Task SeedBasicUserAsync(IUserService userService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        //Seed Default User
        var defaultUser = new IdentityUser
        {
            Id = "11111111-1111-1111-1111-000000000002",
            UserName = "adam.rolitura@gmail.com",
            Email = "adam.rolitura@gmail.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            TenantId = DefaultTenants[0].Id,
        };
        if ((await userManager.Users.ToListAsync()).All(u => u.Id != defaultUser.Id))
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "12345");
                await userManager.AddToRoleAsync(defaultUser, Roles.RegularUser);
            }
        }
    }

    private static async Task SeedSuperAdminAsync(IUserService userService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var defaultUser = new IdentityUser("adambarath@msn.com")
        {
            //Id = "11111111-1111-1111-1111-000000000001",
            Name = "adambarath@msn.com",
            UserName = "adambarath@msn.com",
            Email = "adambarath@msn.com",
            EmailConfirmed = true,
            TenantId = DefaultTenants[0].Id,
        };
        if ((await userManager.Users.ToListAsync()).All(u => u.Id != defaultUser.Id))
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userService.CreateAsync(new() { Email = defaultUser.Email, Name = defaultUser.Name, TenantId = DefaultTenants[0].Id, });
                await userManager.AddToRoleAsync(defaultUser, Roles.RegularUser);
                await userManager.AddToRoleAsync(defaultUser, Roles.TenantAdmin);
                await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin);
            }
            await roleManager.SeedClaimsForSuperAdmin();
        }
    }

    private static async Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
    {
        var adminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin);
        await roleManager.AddPermissionClaim(adminRole, "Identity");
    }

    public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
    {
        var allClaims = await roleManager.GetClaimsAsync(role);
        var allPermissions = DefaultPermissions.GeneratePermissionsForModule(module);

        foreach (var permission in allPermissions)
        {
            if (!allClaims.Any(a => a.Type == Claims.Permission && a.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new Claim(Claims.Permission, permission));
            }
        }
    }
}
