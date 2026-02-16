using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Text;
using AndroidX.Core.App;
using Brupper.Push.Models;
using Java.Lang;
using Java.Util;
using Microsoft.AppCenter.Android.Utils;
using Newtonsoft.Json;
using System;

namespace Brupper.Push.Android
{
    public class PushNotifier
    {
        public const string DefaultChannel = "hu.brupper.default";
        public const string LaunchKey = "launchkey";

        public const string CHANNEL_ID = "azure_push";
        public const string CHANNEL_NAME = "Push";
        public const string DEFAULT_COLOR_METADATA_NAME = "com.google.firebase.messaging.default_notification_color";
        public const string DEFAULT_ICON_METADATA_NAME = "com.google.firebase.messaging.default_notification_icon";

        #region New wave

        public void CreateNotification(Content toast, Context context, System.Type mainActivity, Bitmap largeIcon)
        {
            if (string.IsNullOrEmpty(toast.Title) || string.IsNullOrEmpty(toast.Body))
            {
                return;
            }

            var uiIntent = new Intent(context, mainActivity); // DO NOT USE: other activity will crash MvvmCross framework. Splash is able to handle new intents. ;)
            uiIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop); // + [Activity(, LaunchMode = LaunchMode.SingleInstance)]
            //uiIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask); // ez ujra csinalja az activity-t

            // Add custom args
            if (toast.CustomData != null)
                foreach (var arg in toast.CustomData)
                    uiIntent.PutExtra(arg.Key, arg.Value);

            uiIntent.PutExtra(LaunchKey, JsonConvert.SerializeObject(toast));

            var pendingIntent = PendingIntent.GetActivity(Application.Context, 1, uiIntent, PendingIntentFlags.UpdateCurrent);
            var builder = new NotificationCompat.Builder(context, DefaultChannel)
                .SetContentTitle(toast.Title)
                .SetContentText(toast.Body)
                .SetLargeIcon(largeIcon)
                .SetContentIntent(pendingIntent)
                .SetAutoCancel(true)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(toast.Body))
                .SetPriority((int)NotificationPriority.Max)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification));
            builder.SetSmallIcon(GetNotificationIcon(builder));
            var notification = builder.Build();

            var notificationManager = context.GetSystemService(Application.NotificationService) as NotificationManager;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var importance = NotificationImportance.High;
                NotificationChannel channel = new NotificationChannel(DefaultChannel, PushNotifier.DefaultChannel, importance);
                channel.EnableVibration(false);
                notificationManager.CreateNotificationChannel(channel);
            }

            notificationManager.Notify(notificationId++, notification);
        }

        public static int notificationId = new System.Random().Next(0, 100);

        public static int GetNotificationIcon(NotificationCompat.Builder notificationBuilder)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                notificationBuilder.SetColor(Color.White);
                return Resource.Drawable.ic_stat_notify_dot;
            }

            return Resource.Drawable.ic_stat_notify_dot;
        }

        #endregion

        #region AppCenter

        public static void HandleNotification(Context context, Intent pushIntent)
        {
            Notification.Builder builder;
            Context context2 = context.ApplicationContext;
            NotificationManager notificationManager = (NotificationManager)context2.GetSystemService("notification");
            string messageId = PushIntentUtils.GetMessageId(pushIntent);
            if (messageId == null)
            {
                AppCenterLog.Warn("AppCenterPush", "Push notification did not contain identifier, generate one.");
                messageId = UUID.RandomUUID().ToString();
            }
            int notificationId = messageId.GetHashCode();
            Intent actionIntent = context2.PackageManager.GetLaunchIntentForPackage(context2.PackageName);
            if (actionIntent != null)
            {
                actionIntent.SetFlags((ActivityFlags)335544320);
                var customData = PushIntentUtils.GetCustomData(pushIntent);
                foreach (var key in customData.Keys)
                {
                    actionIntent.PutExtra(key, customData[key]);
                }
                PushIntentUtils.SetMessageId(actionIntent, messageId);
            }
            else
            {
                actionIntent = new Intent();
            }
            string notificationTitle = PushIntentUtils.GetTitle(pushIntent);
            if (string.IsNullOrEmpty(notificationTitle))
            {
                notificationTitle = global::Microsoft.Maui.ApplicationModel.AppInfo.Name;//AppNameHelper.GetAppName(context2);
            }
            string notificationMessage = PushIntentUtils.GetMessage(pushIntent);
            if ((int)Build.VERSION.SdkInt < 26 || (int)context2.ApplicationInfo.TargetSdkVersion < 26)
            {
                builder = getOldNotificationBuilder(context2);
            }
            else
            {
                NotificationChannel channel = new NotificationChannel(CHANNEL_ID, new Java.Lang.String(CHANNEL_NAME), NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(channel);
                builder = new Notification.Builder(context2, channel.Id);
            }
            setColor(context2, pushIntent, builder);
            setIcon(context2, pushIntent, builder);
            setSound(context2, pushIntent, builder);
            builder
                // TODO: .SetWhen()
                .SetContentTitle(notificationTitle)
                .SetContentText(notificationMessage)
                .SetStyle(new Notification.BigTextStyle().BigText(notificationMessage));
            builder.SetContentIntent(PendingIntent.GetActivity(context2, notificationId, actionIntent, 0));
            Notification notification = builder.Build();
            notification.Flags |= NotificationFlags.AutoCancel; // 16;
            notificationManager.Notify(notificationId, notification);
        }

        private static Notification.Builder getOldNotificationBuilder(Context context)
        {
            return new Notification.Builder(context);
        }

        private static void setColor(Context context, Intent pushIntent, Notification.Builder builder)
        {
            if ((int)Build.VERSION.SdkInt >= 21)
            {
                string colorString = PushIntentUtils.GetColor(pushIntent);
                if (colorString != null)
                {
                    try
                    {
                        builder.SetColor(Color.ParseColor(colorString));
                        return;
                    }
                    catch (IllegalArgumentException e)
                    {
                        AppCenterLog.Error("AppCenterPush", "Invalid color string received in push payload.", e);
                    }
                }
                int colorResourceId = getResourceIdFromMetadata(context, DEFAULT_COLOR_METADATA_NAME);
                if (colorResourceId != 0)
                {
                    AppCenterLog.Debug("AppCenterPush", "Using color specified in meta-data for notification.");
                    builder.SetColor(getColor(context, colorResourceId));
                }
            }
        }

        private static void setSound(Context context, Intent pushIntent, Notification.Builder builder)
        {
            string sound = PushIntentUtils.GetSound(pushIntent);
            if (sound != null)
            {
                try
                {
                    var resources = context.Resources;
                    int id = resources.GetIdentifier(sound, "raw", context.PackageName);
                    builder.SetSound(new global::Android.Net.Uri.Builder().Scheme("android.resource").Authority(resources.GetResourcePackageName(id)).AppendPath(resources.GetResourceTypeName(id)).AppendPath(resources.GetResourceEntryName(id)).Build());
                }
                catch (global::Android.Content.Res.Resources.NotFoundException e)
                {
                    AppCenterLog.Warn("AppCenterPush", "Sound file '" + sound + "' not found; falling back to default.");
                    builder.SetDefaults((NotificationDefaults)1);
                }
            }
        }

        private static void setIcon(Context context, Intent pushIntent, Notification.Builder builder)
        {
            string iconString = PushIntentUtils.GetIcon(pushIntent);
            int iconResourceId = 0;
            if (!TextUtils.IsEmpty(iconString))
            {
                var resources = context.Resources;
                string packageName = context.PackageName;
                iconResourceId = resources.GetIdentifier(iconString, "drawable", packageName);
                if (iconResourceId != 0)
                {
                    AppCenterLog.Debug("AppCenterPush", "Found icon resource in 'drawable'.");
                }
                else
                {
                    iconResourceId = resources.GetIdentifier(iconString, "mipmap", packageName);
                    if (iconResourceId != 0)
                    {
                        AppCenterLog.Debug("AppCenterPush", "Found icon resource in 'mipmap'.");
                    }
                }
            }
            if (iconResourceId != 0)
            {
                iconResourceId = validateIcon(context, iconResourceId);
            }
            if (iconResourceId == 0 && (iconResourceId = getResourceIdFromMetadata(context, DEFAULT_ICON_METADATA_NAME)) != 0)
            {
                AppCenterLog.Debug("AppCenterPush", "Using icon specified in meta-data for notification.");
                iconResourceId = validateIcon(context, iconResourceId);
            }
            if (iconResourceId == 0)
            {
                AppCenterLog.Debug("AppCenterPush", "Using application icon as notification icon.");
                iconResourceId = validateIcon(context, context.ApplicationInfo.Icon);
            }
            if (iconResourceId == 0)
            {
                AppCenterLog.Warn("AppCenterPush", "Using 1 pixel icon as fallback for notification.");
                // TODO: iconResourceId = Resources.drawable.ic_stat_notify_dot;
            }
            builder.SetSmallIcon(iconResourceId);
        }

        private static int validateIcon(Context context, int iconResourceId)
        {
            if (iconResourceId == 0)
            {
                int i = iconResourceId;
                return iconResourceId;
            }
            if ((int)Build.VERSION.SdkInt == 26 && (context.GetDrawable(iconResourceId) is AdaptiveIconDrawable))
            {
                AppCenterLog.Error("AppCenterPush", "Adaptive icons make Notification center crash (system process) on Android 8.0 (was fixed on Android 8.1), please update your icon to be non adaptive or please use another icon to push.");
                iconResourceId = 0;
            }
            int i2 = iconResourceId;
            return iconResourceId;
        }

        private static int getResourceIdFromMetadata(Context context, string metadataName)
        {
            Bundle metaData = null;
            try
            {
                metaData = context.PackageManager.GetApplicationInfo(context.PackageName, PackageInfoFlags.MetaData).MetaData;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                AppCenterLog.Error("AppCenterPush", "Package name not found.", e);
            }
            if (metaData != null)
            {
                return metaData.GetInt(metadataName);
            }
            return 0;
        }

        private static int getColor(Context context, int colorResourceId)
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                return context.GetColor(colorResourceId);
            }
            return context.Resources.GetColor(colorResourceId);
        }

        #endregion

    }
}
