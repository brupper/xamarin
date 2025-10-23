using Brupper.AspNetCore.Identity.Entities;
using Brupper.AspNetCore.Models;

namespace Brupper.AspNetCore.Identity.Models;

public class UserListViewModel : ListViewModel<UserEditViewModel>
{
    public List<Tenant> Tenants { get; set; } = new();
}
