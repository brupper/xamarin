using Android.Content;
using Java.Lang;
using Microsoft.AppCenter.Android.Utils;
using System.Collections.Generic;

namespace Brupper.Push.Android
{
    public static class PushIntentUtils
    {
        public static string EXTRA_COLOR = "gcm.notification.color";
        public static string EXTRA_GCM_PREFIX = "gcm.notification.";
        public static string EXTRA_GOOGLE_MESSAGE_ID = "google.message_id";
        public static string EXTRA_GOOGLE_PREFIX = "google.";
        public static string EXTRA_ICON = "gcm.notification.icon";
        public static string EXTRA_MESSAGE = "gcm.notification.body";
        public static string EXTRA_SOUND = "gcm.notification.sound";
        public static string EXTRA_SOUND_ALT = "gcm.notification.sound2";
        public static readonly HashSet<string> EXTRA_STANDARD_KEYS = new HashSet<string>() { "collapse_key", "from" };
        public static string EXTRA_TITLE = "gcm.notification.title";

        public static Dictionary<string, string> GetCustomData(this Intent pushIntent)
        {
            var customData = new Dictionary<string, string>();
            var standardData = new Dictionary<string, string>();
            var intentExtras = pushIntent.Extras;
            if (intentExtras != null)
            {
                foreach (string key in intentExtras.KeySet())
                {
                    string value = String.ValueOf(intentExtras.Get(key));
                    if (key.StartsWith(EXTRA_GCM_PREFIX) || key.StartsWith(EXTRA_GOOGLE_PREFIX) || EXTRA_STANDARD_KEYS.Contains(key))
                    {
                        standardData[key] = value;
                    }
                    else
                    {
                        customData[key] = value;
                    }
                }
            }
            AppCenterLog.Debug("AppCenterPush", "Push standard data: " + standardData);
            AppCenterLog.Debug("AppCenterPush", "Push custom data: " + customData);
            return customData;
        }

        public static string GetTitle(this Intent pushIntent)
        {
            return pushIntent.GetStringExtra(EXTRA_TITLE);
        }

        public static string GetMessage(this Intent pushIntent)
        {
            return pushIntent.GetStringExtra(EXTRA_MESSAGE);
        }

        public static string GetMessageId(this Intent pushIntent)
        {
            return pushIntent.GetStringExtra(EXTRA_GOOGLE_MESSAGE_ID);
        }

        public static void SetMessageId(this Intent pushIntent, string messageId)
        {
            pushIntent.PutExtra(EXTRA_GOOGLE_MESSAGE_ID, messageId);
        }

        public static string GetSound(this Intent pushIntent)
        {
            string sound = pushIntent.GetStringExtra(EXTRA_SOUND_ALT);
            return sound == null ? pushIntent.GetStringExtra(EXTRA_SOUND) : sound;
        }

        public static string GetColor(this Intent pushIntent)
        {
            return pushIntent.GetStringExtra(EXTRA_COLOR);
        }

        public static string GetIcon(this Intent pushIntent)
        {
            return pushIntent.GetStringExtra(EXTRA_ICON);
        }
    }
}
