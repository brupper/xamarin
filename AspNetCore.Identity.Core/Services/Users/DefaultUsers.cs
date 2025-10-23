using Brupper.AspNetCore.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using static Brupper.AspNetCore.Identity.Services.Users.IdentityConstants;
using IdentityUser = Brupper.AspNetCore.Identity.Entities.User;

namespace Brupper.AspNetCore.Identity.Services.Users;

public static class DefaultUsers
{
    #region Default data

    internal const string DefaultPassword = "asdfgh1234";

    internal static List<Tenant> DefaultTenants { get; } = new() {
        new()
        {
            Id = "AAAAAAAA-0000-0000-0000-111111111111",
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
        new()
        {
            Id = "00000000-0000-0000-0000-000000000002",
            PartitionKey = "seed",
            Contact = "Bota Ádám",
            Name = "Volmer-Tours Kft.",
            Email = "info@rolitura.hu",
            Phone = "+36-70-278-6852",
            Zip = "9200",
            City = "Mosonmagyaróvár",
            Address = "Gyári út",
            Number = "29.",
            PostalZip = "9200",
            PostalCity = "Mosonmagyaróvár",
            PostalAddress = "Gyári út",
            PostalNumber = "29.",
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

    internal static List<IdentityUser> DefaultUserSeed { get; } =
    [
        new ("adambarath@msn.com")
        {
            Id = "11111111-1111-1111-1111-000000000001",
            Name = "adambarath@msn.com",
            UserName = "adambarath@msn.com",
            Email = "adambarath@msn.com",
            EmailConfirmed = true,
            TenantId = DefaultTenants[0].Id,
        },
        new ("adam.bota@rolitura.com")
        {
            Id = "11111111-1111-1111-1111-000000000002",
            UserName = "adam.bota@rolitura.com",
            Email = "adam.bota@rolitura.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            TenantId = DefaultTenants[0].Id,
        },
    ];

    #endregion

    public static async Task InitAndSeedDatabaseAsync(this IServiceProvider services)
    {
        using var tenantDb = services.GetService<Contexts.TenantDataContext>()!;
        await tenantDb.Database.EnsureCreatedAsync();

        using var db = services.GetService<IdentityDbContext<User>>()!;
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
            //await SeedBasicUsersAsync(userService, userManager, roleManager);
            //await SeedSuperAdminAsync(userService, userManager, roleManager);

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
        var roles = new HashSet<string>(await roleManager.Roles.Select(x => x.Name!).ToListAsync(), StringComparer.OrdinalIgnoreCase);
        var rolesToInsert = Roles.AllRoles.Where(r => !roles.Contains(r.Name!)).ToList();

        foreach (var identityRole in rolesToInsert)
        {
            await roleManager.CreateAsync(identityRole);
            await roleManager.AddClaimAsync(identityRole, new Claim(Claims.Role, identityRole.Name!));
        }
    }

    public static async Task SeedBasicUsersAsync(IUserService userService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        //Seed Default Users
        foreach (var defaultUser in DefaultUserSeed)
        {
            if ((await userManager.Users.ToListAsync()).All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    // TODO handle tenants: await userService.CreateAsync(new() { Email});
                    await userManager.CreateAsync(defaultUser, DefaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, Roles.RegularUser);
                }
            }
        }
    }

    private static async Task SeedSuperAdminAsync(IUserService userService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var defaultUser = DefaultUserSeed[0];

        var user = await userManager.FindByEmailAsync(defaultUser.Email);
        if (user != null)
        {
            await userManager.AddToRoleAsync(defaultUser, Roles.TenantAdmin);
            await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin);
        }
        await roleManager.SeedClaimsForSuperAdmin();
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
