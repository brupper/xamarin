namespace Plugin.Toasts
{
    /// <summary> forked from: https://github.com/EgorBo/Toasts.Forms.Plugin </summary>
    public enum NotificationStyle
    {
        Default = 0, // Will choose Snackbar when < Lollipop and Notifications when >= Lollipop
        Snackbar = 1,
        Notifications = 2 // Heads-up notifications. These will work on lower than Lollipop but it wont pop up at the top of the screen.
    }
}