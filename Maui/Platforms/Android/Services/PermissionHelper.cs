using Android.App;
using AndroidX.Core.App;
using Brupper.Maui.Services;

namespace Brupper.Maui.Platforms.Android.Services;

internal class PermissionHelper : IPermissionHelper
{
    public bool RegisteredForNotifications()
    {
        var nm = NotificationManagerCompat.From(Application.Context);
        bool enabled = nm.AreNotificationsEnabled();
        return enabled;
    }
}