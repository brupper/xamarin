using Brupper.Maui.Services;
using Microsoft.Maui.ApplicationModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Brupper.Maui.Platforms.iOS;

internal class PermissionHelper : IPermissionHelper
{
    public bool RegisteredForNotifications()
    {
        var types = UIApplication.SharedApplication.CurrentUserNotificationSettings.Types;
        if (types.HasFlag(UIUserNotificationType.Alert))
        {
            return true;
        }

        return false;
    }
}


// https://github.com/trailheadtechnology/NotificationApp/tree/master/NotificationApp

// Extend Xamarin Essentials
internal class PostNotificationsPermission : BasePlatformPermission
{
    /// <inheritdoc/>
    protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
        () => new string[] { "" };
}

// Implementing PostNotificationPermission
public class PostNotificationPermissionService : IPostNotificationPermissionService
{
    // todo: sugar fitness-bol atemelni


    public async Task<bool> CheckAndRequestPermissionsAsync()
    {
        var settings = UIApplication.SharedApplication.CurrentUserNotificationSettings.Types;
        
        
        if (settings.HasFlag(UIUserNotificationType.Alert))
        {
            return true;
        }

        return settings != UIUserNotificationType.None;
    }
}