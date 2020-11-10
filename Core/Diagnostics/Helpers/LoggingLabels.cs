// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    public static class LoggingLabels
    {
        public const string TimeSpentInForegroundLabel = "Time spent in foreground";
        public const string TimeSpentOnCustomPageLabel = "Time spent on {0} page";
        public const string CustomButtonPressedLabel = "{0} button pressed";
        public const string NewVisitWasCancelled = "New visit was cancelled";
        public const string NewVisitPageInit = "New Visit page - loading";
        public const string StartOfCustomTaskLabel = "Starting '{0}' Task...";
        public const string EndOfCustomTaskLabel = "End of '{0}' Task.";
        public const string ConsoleTraceLabel = "[TRACE][{0}] {1}";
        public const string ConsoleEventLabel = "[EVENT] {0}";
        public const string ConsoleStartPageViewLabel = "[START_PAGEVIEW] {0}";
        public const string ConsoleStopPageViewLabel = "[STOP_PAGEVIEW] {0} - {1}";
        public const string ConsoleTrackExceptionLabel = "[EXCEPTION] {0}";
        public const string ConsoleStartTimeTrackLabel = "[START_TIMETRACK] {0}";
        public const string ConsoleStopTimeTrackLabel = "[STOP_TIMETRACK] {0}";
        public const string LocalStorageTraceLabel = "[TRACE][{0:HH:mm:ss}][{1}] {2}{3}";
        public const string LocalStorageEventLabel = "[EVENT][{0:HH:mm:ss}] {1} {2}{3}";
        public const string LocalStorageStartPageViewLabel = "[START_PAGEVIEW][{0:HH:mm:ss}] {1}{2}";
        public const string LocalStorageStopPageViewLabel = "[STOP_PAGEVIEW][{0:HH:mm:ss}] {1} - {2}{3}";
        public const string LocalStorageTrackExceptionLabel = "[EXCEPTION][{0:HH:mm:ss}] {1}{2}";
        public const string LocalStorageStartTimeTrackLabel = "[START_TIMETRACK][{0:HH:mm:ss}] {1}{2}";
        public const string LocalStorageStartTimeTrackWithProcessIdLabel = "[START_TIMETRACK][{0:HH:mm:ss}] {1}{2}  {3}: {4}{5}";
        public const string LocalStorageStopTimeTrackLabel = "[STOP_TIMETRACK][{0:HH:mm:ss}] {1}{2}  {3}: {4}{5}";
        public const string LocalStorageStopTimeTrackWithProcessIdLabel = "[STOP_TIMETRACK][{0:HH:mm:ss}] {1}{2}  {3}: {4}{5}  {6}: {7}{8}";
        public const string SimpleLogDetailLabel = "  {0}: {1}";
        public const string LogFileNameLabel = "_log_{2}_{1}_{0}.txt";
        public const string MethodLabel = "Method";
        public const string GetLabel = "GET";
        public const string PostLabel = "POST";
        public const string PatchLabel = "PATCH";
        public const string HttpResponseDetailsLabel = "Method: {0} Uri: {1} Reason: {2}, version: {3}";
        public const string PhotoIdLabel = "Photo Id";
        public const string ProcessIdLabel = "Process Id";
        public const string EllapsedTimeLabel = "Ellapsed Time";
        public const string ApplicationGoesForegroundLabel = "Application goes foreground";
        public const string ApplicationGoesBackgroundLabel = "Application goes background";
    }
}
