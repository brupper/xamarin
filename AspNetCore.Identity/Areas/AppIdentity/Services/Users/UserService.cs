using System.Security.Claims;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Contexts;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Repositories;
using AspNetCore.Identity.CosmosDb.Stores;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using static Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users.IdentityConstants;
using ICosmosRepository = AspNetCore.Identity.CosmosDb.Contracts.IRepository;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users;

public class UserService : IUserService
{
    #region Fields

    private readonly IdentityDataContext identityDataContext;
    private readonly IUserStore<User> userStore;
    private readonly UserManager<User> userManager;
    private readonly ITenantRepository tenantRepository;
    private readonly ICosmosRepository cosmosRepository;

    #endregion

    #region Constructor

    public UserService(
        IdentityDataContext identityDataContext,
        IUserStore<User> userStore,
        UserManager<User> userManager,
        ITenantRepository tenantRepository,
        ICosmosRepository cosmosRepository
        )
    {
        Guard.IsNotNull(identityDataContext);
        Guard.IsNotNull(userStore);
        Guard.IsNotNull(userManager);
        Guard.IsNotNull(tenantRepository);
        Guard.IsNotNull(cosmosRepository);

        this.identityDataContext = identityDataContext;
        this.userStore = userStore;
        this.userManager = userManager;
        this.tenantRepository = tenantRepository;
        this.cosmosRepository = cosmosRepository;
    }

    #endregion

    public async Task CreateAsync(UserEditViewModel model)
    {
        var existingUser = await userManager.FindByNameAsync(model.Email);
        if (existingUser == null)
        {
            existingUser = new User(model.Email)
            {
                Email = model.Email,
                Name = model.Name,
                TenantId = model.TenantId,
                EmailConfirmed = true,
                // NormalizedEmail = userManager.NormalizeEmail(model.Email),
            };

            await userManager.CreateAsync(existingUser);
            await SetupClaimsAsync(existingUser);

            var tenant = (await tenantRepository.GetAsync(x => x.Id == model.TenantId)).FirstOrDefault();
            if (tenant?.Licences?.Count > 0)
            {
                await SetupLicencesAsync(existingUser, tenant.Licences.Select(x => new TenantModulesViewModel { Name = x.Name, Selected = true }));
            }
        }
    }

    public async Task UpdateAsync(UserEditViewModel model)
    {
        var user = await userManager.FindByIdAsync(model.Id);

        user.Email = model.Email;
        user.Name = model.Name;
        user.TenantId = model.TenantId;

        await userStore.UpdateAsync(user, default);

        if (model.Email != user.UserName)
        {
            await userManager.SetUserNameAsync(user, model.Email);
        }

        await SetupClaimsAsync(user);

        var tenant = (await tenantRepository.GetAsync(x => x.Id == model.TenantId)).FirstOrDefault();
        if (tenant?.Licences?.Count > 0)
        {
            await SetupLicencesAsync(user, tenant.Licences.Select(x => new TenantModulesViewModel { Name = x.Name, Selected = true }));
        }
    }

    public async Task<IEnumerable<User>> GetByTenantIdAsync(string tenantId)
    {
        var query = await identityDataContext.Users.Where(u => u.TenantId == tenantId).ToListAsync();

        return query;
    }

    public async Task SetupLicencesAsync(string tenantId, IEnumerable<TenantModulesViewModel> modules)
    {
        var users = await userManager.Users.Where(x => x.TenantId == tenantId).ToListAsync();
        foreach (var user in users)
        {
            await SetupLicencesAsync(user, modules);
        }
    }

    public async Task SetupLicencesAsync(User user, IEnumerable<TenantModulesViewModel> modules)
    {
        var claims = (await userManager.GetClaimsAsync(user)).Where(x => x.Type is Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users.IdentityConstants.ModuleClaimType).ToList();
        if (claims.Any())
        {
            await userManager.RemoveClaimsAsync(user, claims);
        }

        var newClaimList = modules
                .Where(x => x.Selected)
                .Select(x => new Claim(ModuleClaimType, x.Name))
                .ToList();
        if (modules.Any(x => x.Selected))
        {
            await userManager.AddClaimsAsync(user, newClaimList);
        }

        // ha van elteres:
        if (!ClaimsAreEqual(claims, newClaimList))
        {
            // needs SecurityStampValidatorOptions => ValidationInterval = TimeSpan.Zero; // enables immediate logout, after updating the user's stat.
            await userManager.UpdateSecurityStampAsync(user);
        }
    }

    private async Task SetupClaimsAsync(User user)
    {
        var claims = (await userManager.GetClaimsAsync(user)).Where(x => x.Type is DisplayNameClaimType or TenantClaimType).ToList();
        if (claims.Any())
        {
            await userManager.RemoveClaimsAsync(user, claims);
        }

        var newClaimList = new[]
        {
            new Claim(DisplayNameClaimType, user.Name),
            new Claim(TenantClaimType, user.TenantId),
        };
        await userManager.AddClaimsAsync(user, newClaimList);

        // ha van elteres:
        if (!ClaimsAreEqual(claims, newClaimList))
        {
            // needs SecurityStampValidatorOptions => ValidationInterval = TimeSpan.Zero; // enables immediate logout, after updating the user's stat.
            await userManager.UpdateSecurityStampAsync(user);
        }
    }

    public async Task LockUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        var lockUser = await userManager.SetLockoutEnabledAsync(user, true);
        var lockDate = await userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(5));

        await userManager.UpdateSecurityStampAsync(user); // forces logout of the user
    }

    public async Task UnlockUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        var lockUser = await userManager.SetLockoutEnabledAsync(user, false);
        var lockDate = await userManager.SetLockoutEndDateAsync(user, null);
    }

    public async Task RemoveUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        var claims = await userManager.GetClaimsAsync(user);
        if (claims.Any())
        {
            await userManager.RemoveClaimsAsync(user, claims);
        }

        await userManager.DeleteAsync(user);
    }

    private static bool ClaimsAreEqual(IEnumerable<Claim> claims1, IEnumerable<Claim> claims2)
        => claims1.Select(x => x.Value).OrderBy(x => x).SequenceEqual(claims2.Select(x => x.Value).OrderBy(x => x));
}
