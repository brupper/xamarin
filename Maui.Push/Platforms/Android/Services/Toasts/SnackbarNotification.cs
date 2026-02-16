using Android.App;
using Android.Text;
using Android.Views;
using Google.Android.Material.Snackbar;
using Plugin.Toasts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Plugin.Toasts
{
    /// <summary> forked from: https://github.com/EgorBo/Toasts.Forms.Plugin </summary>
    public class SnackbarNotification
    {
        private readonly IDictionary<string, ManualResetEvent> resetEvents = new ConcurrentDictionary<string, ManualResetEvent>();
        private readonly IDictionary<string, NotificationResult> eventResult = new ConcurrentDictionary<string, NotificationResult>();
        private readonly IList<Snackbar> snackBars = new List<Snackbar>();

        private int count;
        private readonly object @lock = new object();

        public NotificationResult Notify(Activity activity, INotificationOptions options)
        {
            var view = activity.FindViewById(global::Android.Resource.Id.Content);

            SpannableStringBuilder builder = new SpannableStringBuilder();

            builder.Append(options.Title);

            if (!string.IsNullOrEmpty(options.Title) && !string.IsNullOrEmpty(options.Description))
                builder.Append("\n"); // Max of 2 lines for snackbar

            builder.Append(options.Description);

            var id = count.ToString();
            count++;

            var snackbar = Snackbar.Make(view, builder, Snackbar.LengthLong);
            if (options.IsClickable)
                snackbar.SetAction(options.AndroidOptions.ViewText, new EmptyOnClickListener(id, ToastClosed, new NotificationResult { Action = NotificationAction.Clicked }));
            else
                snackbar.SetAction(options.AndroidOptions.DismissText, new EmptyOnClickListener(id, ToastClosed, new NotificationResult { Action = NotificationAction.Dismissed }));

            // Monitor callbacks
            snackbar.SetCallback(new ToastCallback(id, ToastClosed));

            // Setup reset events
            var resetEvent = new ManualResetEvent(false);
            resetEvents.Add(id, resetEvent);
            snackBars.Add(snackbar);
            snackbar.Show();

            resetEvent.WaitOne(); // Wait for a result

            var notificationResult = eventResult[id];

            eventResult.Remove(id);
            resetEvents.Remove(id);

            if (snackBars.Contains(snackbar))
                snackBars.Remove(snackbar);

            return notificationResult;
        }

        public void CancelAll()
        {
            foreach (var snackbar in snackBars)
                snackbar.Dismiss();
        }

        private void ToastClosed(string id, NotificationResult result)
        {
            lock (@lock)
            {
                if (resetEvents.ContainsKey(id))
                {
                    eventResult.Add(id, result);
                    resetEvents[id].Set();
                }
            }
        }
    }

    internal class ToastCallback : Snackbar.Callback
    {
        private readonly string id;
        private readonly Action<string, NotificationResult> callback;

        public ToastCallback(string id, Action<string, NotificationResult> callback)
        {
            this.id = id;
            this.callback = callback;
        }

        public override void OnDismissed(Snackbar snackbar, int evt)
        {
            switch (evt)
            {
                case DismissEventAction:
                    return;  // Handled via OnClickListeners
                case DismissEventConsecutive:
                case DismissEventManual:
                case DismissEventSwipe:
                    callback(id, new NotificationResult() { Action = NotificationAction.Dismissed });
                    break;
                case DismissEventTimeout:
                default:
                    callback(id, new NotificationResult() { Action = NotificationAction.Timeout });
                    break;
            }
        }
    }

    internal class EmptyOnClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private readonly string id;
        private readonly Action<string, NotificationResult> callback;

        private readonly NotificationResult result;

        public EmptyOnClickListener(string id, Action<string, NotificationResult> callback, NotificationResult result)
        {
            this.id = id;
            this.callback = callback;
            this.result = result ?? new NotificationResult { Action = NotificationAction.Dismissed };
        }
        public void OnClick(View v)
        {
            callback(id, result);
        }
    }
}