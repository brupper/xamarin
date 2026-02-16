using Newtonsoft.Json;
using System.Collections.Generic;

// // https://github.com/ndrwtrsk/fcm-json-schema/blob/master/fcm-android-payload.json
namespace Brupper.Push.Models
{
    /// <summary>
    /// As per https://firebase.google.com/docs/cloud-messaging/http-server-ref
    /// </summary>
    public partial class FcmPayLoad
    {
        /// <summary>
        /// This parameter identifies a group of messages (e.g., with collapse_key: "Updates
        /// Available") that can be collapsed, so that only the last message gets sent when delivery
        /// can be resumed. This is intended to avoid sending too many of the same messages when the
        /// device comes back online or becomes active (see delay_while_idle). Note that there is no
        /// guarantee of the order in which messages get sent. Note: A maximum of 4 different
        /// collapse keys is allowed at any given time. This means a FCM connection server can
        /// simultaneously store 4 different send-to-sync messages per client app. If you exceed this
        /// number, there is no guarantee which 4 collapse keys the FCM connection server will keep.
        /// </summary>
        [JsonProperty("collapse_key", NullValueHandling = NullValueHandling.Ignore)]
        public string CollapseKey { get; set; }

        /// <summary>
        /// This parameter specifies a logical expression of conditions that determine the message
        /// target. Supported condition: Topic, formatted as "'yourTopic' in topics". This value is
        /// case-insensitive. Supported operators: &&, ||. Maximum two operators per topic message
        /// supported.
        /// </summary>
        [JsonProperty("condition", NullValueHandling = NullValueHandling.Ignore)]
        public string Condition { get; set; }

        /// <summary>
        /// On iOS, use this field to represent content-available in the APNS payload. When a
        /// notification or message is sent and this is set to true, an inactive client app is
        /// awoken. On Android, data messages wake the app by default. On Chrome, currently not
        /// supported.
        /// </summary>
        [JsonProperty("content_available", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ContentAvailable { get; set; }

        /// <summary>
        /// This parameter specifies the custom key-value pairs of the message's payload. For
        /// example, with data:{"score":"3x1"}: On iOS, if the message is sent via APNS, it
        /// represents the custom data fields. If it is sent via FCM connection server, it would be
        /// represented as key value dictionary in AppDelegate
        /// application:didReceiveRemoteNotification:. On Android, this would result in an intent
        /// extra named score with the string value 3x1. The key should not be a reserved word
        /// ("from" or any word starting with "google" or "gcm"). Do not use any of the words defined
        /// in this table (such as collapse_key). Values in string types are recommended. You have to
        /// convert values in objects or other non-string data types (e.g., integers or booleans) to
        /// string.
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Data { get; set; }

        /// <summary>
        /// When this parameter is set to true, it indicates that the message should not be sent
        /// until the device becomes active. The default value is false.
        /// </summary>
        [JsonProperty("delay_while_idle", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DelayWhileIdle { get; set; }

        /// <summary>
        /// This parameter, when set to true, allows developers to test a request without actually
        /// sending a message. The default value is false.
        /// </summary>
        [JsonProperty("dry_run", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DryRun { get; set; }

        /// <summary>
        /// This parameter specifies the predefined, user-visible key-value pairs of the notification
        /// payload. See Notification payload support for detail. For more information about
        /// notification message and data message options, see Payload -
        /// https://firebase.google.com/docs/cloud-messaging/concept-options#notifications_and_data_messages
        /// </summary>
        [JsonProperty("notification", NullValueHandling = NullValueHandling.Ignore)]
        public NotificationSchema Notification { get; set; }

        /// <summary>
        /// Sets the priority of the message. Valid values are "normal" and "high." On iOS, these
        /// correspond to APNs priorities 5 and 10. By default, messages are sent with normal
        /// priority. Normal priority optimizes the client app's battery consumption and should be
        /// used unless immediate delivery is required. For messages with normal priority, the app
        /// may receive the message with unspecified delay. When a message is sent with high
        /// priority, it is sent immediately, and the app can wake a sleeping device and open a
        /// network connection to your server. For more information, see Setting the priority of a
        /// message.
        /// </summary>
        [JsonProperty("priority", NullValueHandling = NullValueHandling.Ignore)]
        public string Priority { get; set; }

        /// <summary>
        /// This parameter specifies a list of devices (registration tokens, or IDs) receiving a
        /// multicast message. It must contain at least 1 and at most 1000 registration tokens. Use
        /// this parameter only for multicast messaging, not for single recipients. Multicast
        /// messages (sending to more than 1 registration tokens) are allowed using HTTP JSON format
        /// only.
        /// </summary>
        [JsonProperty("registration_ids")]
        public string[] RegistrationIds { get; set; }

        /// <summary>
        /// This parameter specifies the package name of the application where the registration
        /// tokens must match in order to receive the message.
        /// </summary>
        [JsonProperty("restricted_package_name", NullValueHandling = NullValueHandling.Ignore)]
        public string RestrictedPackageName { get; set; }

        /// <summary>
        /// This parameter specifies how long (in seconds) the message should be kept in FCM storage
        /// if the device is offline. The maximum time to live supported is 4 weeks, and the default
        /// value is 4 weeks. For more information, see Setting the lifespan of a message.
        /// </summary>
        [JsonProperty("time_to_live", NullValueHandling = NullValueHandling.Ignore)]
        public long? TimeToLive { get; set; }

        /// <summary>
        /// This parameter specifies the recipient of a message. The value must be a registration
        /// token, notification key, or topic. Do not set this field when sending to multiple topics.
        /// See condition.
        /// </summary>
        [JsonProperty("to", NullValueHandling = NullValueHandling.Ignore)]
        public string To { get; set; }
    }

    /// <summary>
    /// This parameter specifies the predefined, user-visible key-value pairs of the notification
    /// payload. See Notification payload support for detail. For more information about
    /// notification message and data message options, see Payload -
    /// https://firebase.google.com/docs/cloud-messaging/concept-options#notifications_and_data_messages
    /// </summary>
    public partial class NotificationSchema
    {
        /// <summary>
        /// Indicates notification body text.
        /// </summary>
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public string Body { get; set; }

        /// <summary>
        /// Indicates the string value to replace format specifiers in the body string for
        /// localization. For more information, see Formatting and Styling -
        /// http://developer.android.com/guide/topics/resources/string-resource.html#FormattingAndStyling
        /// </summary>
        [JsonProperty("body_loc_args", NullValueHandling = NullValueHandling.Ignore)]
        public string BodyLocArgs { get; set; }

        /// <summary>
        /// Indicates the key to the body string for localization. Use the key in the app's string
        /// resources when populating this value.
        /// </summary>
        [JsonProperty("body_loc_key", NullValueHandling = NullValueHandling.Ignore)]
        public string BodyLocKey { get; set; }

        /// <summary>
        /// Indicates the action associated with a user click on the notification. When this is set,
        /// an activity with a matching intent filter is launched when user clicks the notification.
        /// </summary>
        [JsonProperty("click_action", NullValueHandling = NullValueHandling.Ignore)]
        public string ClickAction { get; set; }

        /// <summary>
        /// Indicates color of the icon, expressed in #rrggbb format
        /// </summary>
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }

        /// <summary>
        /// Indicates notification icon. Sets value to myicon for drawable resource myicon.
        /// </summary>
        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string Icon { get; set; }

        /// <summary>
        /// Indicates a sound to play when the device receives a notification. Supports default or
        /// the filename of a sound resource bundled in the app. Sound files must reside in /res/raw/.
        /// </summary>
        [JsonProperty("sound", NullValueHandling = NullValueHandling.Ignore)]
        public string Sound { get; set; }

        /// <summary>
        /// Indicates whether each notification results in a new entry in the notification drawer on
        /// Android. If not set, each request creates a new notification. If set, and a notification
        /// with the same tag is already being shown, the new notification replaces the existing one
        /// in the notification drawer.
        /// </summary>
        [JsonProperty("tag", NullValueHandling = NullValueHandling.Ignore)]
        public string Tag { get; set; }

        /// <summary>
        /// Indicates notification title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Indicates the string value to replace format specifiers in the title string for
        /// localization. For more information, see Formatting strings.
        /// </summary>
        [JsonProperty("title_loc_args", NullValueHandling = NullValueHandling.Ignore)]
        public string[] TitleLocArgs { get; set; }

        /// <summary>
        /// Indicates the key to the title string for localization. Use the key in the app's string
        /// resources when populating this value.
        /// </summary>
        [JsonProperty("title_loc_key", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleLocKey { get; set; }
    }
}
