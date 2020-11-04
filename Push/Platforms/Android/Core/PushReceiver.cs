using Android.App;
using Android.Content;
using Android.Runtime;

namespace Brupper.Push.Android
{
    [Preserve]
    [BroadcastReceiver(Permission = "com.google.android.c2dm.permission.SEND")]
    [IntentFilter(new[] { INTENT_ACTION_REGISTRATION, INTENT_ACTION_RECEIVE }, Categories = new[] { "${applicationId}" })]
    public class PushReceiver : BroadcastReceiver//Com.Microsoft.Appcenter.Push.PushReceiver
    {
        public const string INTENT_ACTION_RECEIVE = "com.google.android.c2dm.intent.RECEIVE";
        public const string INTENT_ACTION_REGISTRATION = "com.google.android.c2dm.intent.REGISTRATION";
        public const string INTENT_EXTRA_REGISTRATION = "registration_id";

        public override void OnReceive(Context context, Intent intent)
        {
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
