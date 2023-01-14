using Android.App;
using Android.Content;
using Android.Runtime;
using System.Linq;

namespace Brupper.Push.Android
{
    [Preserve]
    [Service(Exported = true)]
    [BroadcastReceiver(Permission = "com.google.android.c2dm.permission.SEND", Exported = true)]
    [IntentFilter(new[] { INTENT_ACTION_REGISTRATION, INTENT_ACTION_RECEIVE, INTENT_ACTION_MESSAGING }, Categories = new[] { "${applicationId}", })]
    public class PushReceiver : BroadcastReceiver//Com.Microsoft.Appcenter.Push.PushReceiver
    {
        public const string INTENT_ACTION_RECEIVE = "com.google.android.c2dm.intent.RECEIVE";
        public const string INTENT_ACTION_REGISTRATION = "com.google.android.c2dm.intent.REGISTRATION";
        public const string INTENT_ACTION_MESSAGING = "com.google.firebase.MESSAGING_EVENT";
        public const string INTENT_EXTRA_REGISTRATION = "registration_id";

        public override void OnReceive(Context context, Intent intent)
        {
            // Push Notification arrived - print out the keys/values
            var keys = intent.Extras?.KeySet()?.ToList() ?? new System.Collections.Generic.List<string>();
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    global::Android.Util.Log.Debug("AZURE-NOTIFICATION-HUBS", "PushReceiver Key: {0}, Value: {1}", key, intent.Extras.Get(key));
                }
            }

            var action = intent.Action;
            if (INTENT_ACTION_REGISTRATION.Equals(action))
            {
                //AndroidPush.Instance.OnTokenRefresh(intent.GetStringExtra(INTENT_EXTRA_REGISTRATION));
            }
            else if (INTENT_ACTION_RECEIVE.Equals(action))
            {
                //AndroidPush.Instance.OnMessageReceived(context, intent);
            }
        }
    }
}
