using Android.App;
using AndroidX.Core.App;

namespace Brupper.Forms.Platforms.Android.Services
{
    internal class PermissionHelper : IPermissionHelper
    {
        public bool RegisteredForNotifications()
        {
            var nm = NotificationManagerCompat.From(Application.Context);
            bool enabled = nm.AreNotificationsEnabled();
            return enabled;
        }
    }
}