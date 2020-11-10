using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brupper.Identity.B2C
{
    public interface IUserGraphManager
    {
        Task<IEnumerable<User>> ListUsersAsync();
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserBySignInNameAsync(string userId);
        Task DeleteUserByIdAsync(string userId);

        Task<bool> IsLicenseAdministrator(string userId);
    }
}
