using Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Services;

public interface IUserService
{
    Task CreateAsync(UserEditViewModel input);

    Task UpdateAsync(UserEditViewModel input);

    Task LockUserAsync(string userId);

    Task UnlockUserAsync(string userId);

    Task RemoveUserAsync(string userId);

    Task<IEnumerable<User>> GetByTenantIdAsync(string tenantId);

    Task SetupLicencesAsync(string tenantId, IEnumerable<TenantModulesViewModel> modules);

    Task SetupLicencesAsync(User user, IEnumerable<TenantModulesViewModel> modules);
}
