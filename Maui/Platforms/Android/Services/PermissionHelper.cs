using Android.App;
using Android.OS;
using AndroidX.Core.App;
using Brupper.Maui.Services;
using Microsoft.Maui.ApplicationModel;
using static Microsoft.Maui.ApplicationModel.Permissions;
using System.Collections.Generic;
using System.Threading.Tasks;

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


// https://github.com/trailheadtechnology/NotificationApp/tree/master/NotificationApp

// Extend Xamarin Essentials
internal class PostNotificationsPermission : BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new List<(string androidPermission, bool isRuntime)>
        {
             (global::Android.Manifest.Permission.PostNotifications, true)
        }.ToArray();
}

// Implementing PostNotificationPermission
public class PostNotificationPermissionService : IPostNotificationPermissionService
{
    public async Task<bool> CheckAndRequestPermissionsAsync()
    {
        // Tiramisu is Android v13
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        {
            var status = await CheckStatusAsync<PostNotificationsPermission>();

            if (status == PermissionStatus.Granted)
            {
                return true;
            }

            status = await RequestAsync<PostNotificationsPermission>();

            return status == PermissionStatus.Granted;
        }
        return true;
    }
}