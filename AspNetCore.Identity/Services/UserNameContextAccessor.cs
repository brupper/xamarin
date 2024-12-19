using Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Brupper.AspNetCore.Identity.Services;

public class UserNameContextAccessor(IServiceProvider serviceProvider) : IUserContextAccessor
{
    protected readonly IServiceProvider serviceProvider = serviceProvider;

    public string Name
    {
        get
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var httpContextAccessor = scope.ServiceProvider.GetService<IHttpContextAccessor>();
                var user = httpContextAccessor?.HttpContext?.User;

                return user?.Identity?.Name ?? user?.GetEmail();
            }
        }
    }

    public string Email
    {
        get
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var httpContextAccessor = scope.ServiceProvider.GetService<IHttpContextAccessor>();
                var user = httpContextAccessor?.HttpContext?.User;

                return user?.GetEmail();
            }
        }
    }

    public string Id
    {
        get
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var httpContextAccessor = scope.ServiceProvider.GetService<IHttpContextAccessor>();
                var user = httpContextAccessor?.HttpContext?.User;

                return user.GetUserId();
            }
        }
    }

    public string TenantId
    {
        get
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var httpContextAccessor = scope.ServiceProvider.GetService<IHttpContextAccessor>();
                var user = httpContextAccessor?.HttpContext?.User;

                return user?.GetTenant();
            }
        }
    }

    public bool IsNotSuperAdmin
    {
        get
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var httpContextAccessor = scope.ServiceProvider.GetService<IHttpContextAccessor>();
                var user = httpContextAccessor?.HttpContext?.User;

                var isSuperAdmin = user?.Identities?.Any(x => x.IsSuperAdmin()) ?? false;

                return !isSuperAdmin;
            }
        }
    }
}
