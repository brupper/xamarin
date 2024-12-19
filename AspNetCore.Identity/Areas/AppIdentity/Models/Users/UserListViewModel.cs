using Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;
using Brupper.AspNetCore.Models;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;

public class UserListViewModel : ListViewModel<UserEditViewModel>
{
    public List<Tenant> Tenants { get; set; } = new();
}
