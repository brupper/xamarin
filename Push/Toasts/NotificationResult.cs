namespace Plugin.Toasts
{
    /// <summary> forked from: https://github.com/EgorBo/Toasts.Forms.Plugin </summary>
    public class NotificationResult : INotificationResult
    {
        public NotificationAction Action { get; set; }
        public int Id { get; set; }
    }
}
