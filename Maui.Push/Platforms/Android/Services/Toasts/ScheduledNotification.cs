using System;

namespace Plugin.Toasts
{
    /// <summary> forked from: https://github.com/EgorBo/Toasts.Forms.Plugin </summary>
    public class ScheduledNotification
    {
        public string Description { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public bool IsClickable { get; set; } = false;

        public AndroidOptions AndroidOptions { get; set; } = new AndroidOptions();

        public bool ClearFromHistory { get; set; } = false;

        public DateTime? DelayUntil { get; set; } = null;
    }
}