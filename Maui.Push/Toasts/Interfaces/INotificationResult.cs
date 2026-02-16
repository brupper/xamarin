namespace Plugin.Toasts
{
    /// <summary> forked from: https://github.com/EgorBo/Toasts.Forms.Plugin </summary>
    public interface INotificationResult
    {
        NotificationAction Action { get; set; }
        int Id { get; set; }
    }
}
