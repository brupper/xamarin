using Microsoft.Graph;
using Task = System.Threading.Tasks.Task;

namespace Brupper.AspNetCore.Identity.B2C;

public interface IUserGraphManager
{
    Task<IEnumerable<User>> ListUsersAsync();
    Task<User?> GetUserByIdAsync(string userId);
    Task<User?> GetUserBySignInNameAsync(string userId);
    Task DeleteUserByIdAsync(string userId);

    Task<bool> IsLicenseAdministratorAsync(string userId);

    Task<bool> IsUserInRoleAsync(string userId, string role);
}
