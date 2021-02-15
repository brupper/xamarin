namespace Plugin.Toasts
{
    /// <summary> forked from: https://github.com/EgorBo/Toasts.Forms.Plugin </summary>
    public class PlatformOptions : IPlatformOptions
    {
        public int? SmallIconDrawable { get; set; }

        public NotificationStyle Style { get; set; }
    }
}