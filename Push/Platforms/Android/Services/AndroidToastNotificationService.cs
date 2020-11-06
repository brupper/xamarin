using Android.App;
using Android.OS;
using Brupper.Push.Android;
using Brupper.Push.Models;
using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.Push.Platforms.Android.Services
{
    public class AndroidToastNotificationService :
        //ToastNotification, 
        IToastNotificationService, IToastNotificator
    {
        #region IToastNotificator

        static Activity activity;
        static IPlatformOptions androidOptions;
        static SnackbarNotification snackbarNotification;
        static NotificationBuilder notificationBuilder;

        public static void Init(Activity activity, IPlatformOptions androidOptions = null)
        {
            AndroidToastNotificationService.activity = activity;
            snackbarNotification = new SnackbarNotification();
            notificationBuilder = new NotificationBuilder();

            AndroidToastNotificationService.androidOptions =
                androidOptions ?? new PlatformOptions { Style = NotificationStyle.Default, SmallIconDrawable = Resource.Drawable.ic_stat_notify_dot };

            notificationBuilder.Init(AndroidToastNotificationService.androidOptions);
        }

        public Task<INotificationResult> Notify(INotificationOptions options)
        {
            return Task.Run(() =>
            {
                switch (androidOptions.Style)
                {
                    case NotificationStyle.Notifications:
                        return notificationBuilder.Notify(activity, options);
                    case NotificationStyle.Snackbar:
                        return snackbarNotification.Notify(activity, options);
                    default:
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                            return notificationBuilder.Notify(activity, options);
                        else
                            return snackbarNotification.Notify(activity, options);
                }
            });
        }

        public void Notify(Action<INotificationResult> callback, INotificationOptions options)
        {
            Task.Run(async () => await Notify(options)).ContinueWith(task => callback?.Invoke(task.Result));
        }

        public void CancelAllDelivered()
        {
            notificationBuilder.CancelAll();
            snackbarNotification.CancelAll();
        }

        /// <summary>
        /// Available on >= API23 (Android 6.0) as is.
        /// Not Available on >= API23, will return empty list
        /// </summary>
        /// <returns></returns>
        public Task<IList<INotification>> GetDeliveredNotifications()
        {
            return Task.FromResult(notificationBuilder.GetDeliveredNotifications());
        }

        public void SystemEvent(object args)
        {

        }

        #endregion

        #region IToastNotificationService

        public async Task CreateToastAsync(Content hint, Func<Task> tappedAction)
        {
            var options = new NotificationOptions
            {
                IsClickable = true,
                Title = hint.Title,
                Description = hint.Body,
                AllowTapInNotificationCenter = true,
            };
            options.AndroidOptions = new AndroidOptions
            {
                SmallDrawableIcon = Resource.Drawable.ic_stat_notify_dot,
            };

            var notJavaDictionary = hint.CustomData.ToDictionary(k => k.Key, v => v.Value);
            var customArguments = Newtonsoft.Json.JsonConvert.SerializeObject(notJavaDictionary);
            options.CustomArgs.Add(PushNotifier.LaunchKey, customArguments);

            var result = await Notify(options);

            if (tappedAction != null && result.Action == NotificationAction.Clicked)
            {
                await tappedAction();
            }
        }

        #endregion
    }
}