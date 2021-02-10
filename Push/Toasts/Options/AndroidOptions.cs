using Plugin.Toasts.Options;

namespace Plugin.Toasts
{
    /// <summary> forked from: https://github.com/EgorBo/Toasts.Forms.Plugin </summary>
    public class AndroidOptions : IAndroidOptions
    {
        /// <summary>
        /// Applicable only to Notification.Builder, if you want to replace the small icon, you must place an image in your drawables folder and pass the int through here. e.g. Resources.Drawable.MyNewIcon
        /// </summary>
        public int? SmallDrawableIcon { get; set; }
        public string DismissText { get; set; } = "Dismiss";
        public string ViewText { get; set; } = "View";
        public string HexColor { get; set; } = "#FFFFFFFF";
        public bool ForceOpenAppOnNotificationTap { get; set; }
        public AndroidChannelOptions ChannelOptions { get; set; } = new AndroidChannelOptions();
    }
}
