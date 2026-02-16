using System.Threading.Tasks;

namespace Brupper.Maui.Services;

public interface IPermissionHelper
{
    bool RegisteredForNotifications();
}

public interface IPostNotificationPermissionService
{
    Task<bool> CheckAndRequestPermissionsAsync();
}