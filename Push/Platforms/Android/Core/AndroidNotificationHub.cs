using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Brupper.Push.Android;
using Com.Microsoft.Appcenter.Utils;
using Java.Lang;
using AndroidInstallation = WindowsAzure.Messaging.NotificationHubs.Installation;
using AndroidInstallationTemplate = WindowsAzure.Messaging.NotificationHubs.InstallationTemplate;
using AndroidNotificationHub = WindowsAzure.Messaging.NotificationHubs.NotificationHub;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
namespace Microsoft.Azure.NotificationHubs.Client
{
    public partial class NotificationHub
    {
        static string LOG_TAG = "NotificationHub";
        private static string PUSH_GROUP = "group_push";
        private static string SERVICE_NAME = "Push";

        public static void CheckLaunchedFromNotification(Activity activity, Intent intent)
        {
            if (activity == null)
            {
                AppCenterLog.Error(LOG_TAG, "Push.checkLaunchedFromNotification: activity may not be null");
            }
            else if (intent == null)
            {
                AppCenterLog.Error(LOG_TAG, "Push.checkLaunchedFromNotification: intent may not be null");
            }
            else
            {
                //postOnUiThread(new Runnable() {
                //    public void run() {
                CheckPushInIntent(activity, intent);
                //    }
                //});
            }
        }

        public static void CheckPushInIntent(Activity activity, Intent intent)
        {

        }
    }
}
