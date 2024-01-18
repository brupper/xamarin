using Brupper.Maui.Services;
using UIKit;

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