using Brupper.Push.Models;
using Plugin.Toasts;
using Plugin.Toasts.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;

namespace Brupper.Push.Platforms.iOS.Services
{
    public class AppleToastNotificationService
       : IToastNotificationService, IToastNotificator
    {
        #region IToastNotificator

        private UNNotificationManager notificationManager;
        private LocalNotificationManager localNotificationManager;

        public AppleToastNotificationService()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                notificationManager = new UNNotificationManager();
            else
                localNotificationManager = new LocalNotificationManager();
        }

        public static void Init() { }

        public Task<INotificationResult> Notify(INotificationOptions options)
        {
            INotificationResult result = null;
            return Task.Run(() =>
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                    return notificationManager.Notify(options);
                else
                {
                    ManualResetEvent reset = new ManualResetEvent(false);
                    UIApplication.SharedApplication.InvokeOnMainThread(() => { result = localNotificationManager.Notify(options); reset.Set(); });
                    reset.WaitOne();
                    return result;
                }
            });
        }

        public void Notify(Action<INotificationResult> callback, INotificationOptions options)
        {
            Task.Run(async () => callback(await Notify(options)));
        }

        public async Task<IList<INotification>> GetDeliveredNotifications()
        {
            var list = new List<INotification>();

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                var notificationCenter = UNUserNotificationCenter.Current;

                var deliveredNotifications = await notificationCenter.GetDeliveredNotificationsAsync();

                foreach (var notification in deliveredNotifications)
                {
                    var content = notification.Request.Content;
                    list.Add(new Notification()
                    {
                        Id = notification.Request.Identifier,
                        Title = content.Title,
                        Description = content.Body,
                        Delivered = notification.Date.ToDateTime()
                    });
                }
            }

            return list;
        }

        public void CancelAllDelivered()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                var notificationCenter = UNUserNotificationCenter.Current;

                notificationCenter.RemoveAllDeliveredNotifications();
            }
        }

        public void SystemEvent(object args)
        {
            // ignore
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
            options.CustomArgs.Add("LaunchKey", Newtonsoft.Json.JsonConvert.SerializeObject(hint.CustomData));

            var result = await Notify(options);

            if (result.Action == NotificationAction.Clicked)
            {
                await tappedAction();
            }
        }

        #endregion
    }
}