using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    public class LocalStorageLogProvider : TextLogProvider, ILocalStorageLogProvider
    {
        public static readonly string LocalLogFolder = "logs";

        public string Folder { get; }

        public LocalStorageLogProvider()
        {
            Folder = LocalLogFolder;
        }

        private string GetFilePathFromDate(DateTime date)
        {
            return Path.Combine(Folder, string.Format(LoggingLabels.LogFileNameLabel, date.Day, date.Month, date.Year));
        }

        public Task<string> ReadLogFile(bool deleteAfterRead = false)
        {
            return DiagnosticsStorage.ReadAllTextAsync(GetFilePathFromDate(DateTime.Today));
        }

        public override Task TrackTraceAsync(LogMessage message)
        {
            var stringMessage = PrepareStringMessage(message);

            return DiagnosticsStorage.AppendTextAsync(
                GetFilePathFromDate(DateTime.Today),
                string.Format(LoggingLabels.LocalStorageTraceLabel, message.OccurredOn, message.Tag, stringMessage, Environment.NewLine));
        }

        public override Task TrackEventAsync(LogMessage message)
        {
            var stringMessage = PrepareStringMessage(message);

            return DiagnosticsStorage.AppendTextAsync(
                GetFilePathFromDate(DateTime.Today),
                string.Format(LoggingLabels.LocalStorageEventLabel, message.OccurredOn, message.EventName, stringMessage, Environment.NewLine));
        }

        public override Task StartTrackPageViewAsync(string pageName, IEnumerable<KeyValuePair<string, string>>? metaData = null)
        {
            TrackTimeHandler.Start(pageName);

            return DiagnosticsStorage.AppendTextAsync(
                GetFilePathFromDate(DateTime.Today),
                string.Format(LoggingLabels.LocalStorageStartPageViewLabel, DateTime.UtcNow, pageName, Environment.NewLine));
        }

        public override Task StopTrackPageViewAsync(string pageName)
        {
            var ellapsedTime = TrackTimeHandler.Stop(pageName);

            return DiagnosticsStorage.AppendTextAsync(
                GetFilePathFromDate(DateTime.Today),
                string.Format(LoggingLabels.LocalStorageStopPageViewLabel, DateTime.UtcNow, pageName, ellapsedTime, Environment.NewLine));
        }

        public override Task TrackExceptionAsync(LogMessage message)
        {
            var stringMessage = PrepareStringMessage(message);

            return DiagnosticsStorage.AppendTextAsync(
                GetFilePathFromDate(DateTime.Today),
                string.Format(LoggingLabels.LocalStorageTrackExceptionLabel, message.OccurredOn, stringMessage, Environment.NewLine));
        }

        public override Task StartTrackTimeAsync(string key, string? processId = null)
        {
            TrackTimeHandler.Start(key, processId);

            if (processId != null)
            {
                return DiagnosticsStorage.AppendTextAsync(
                    GetFilePathFromDate(DateTime.Today),
                    string.Format(LoggingLabels.LocalStorageStartTimeTrackWithProcessIdLabel, DateTime.UtcNow, key, Environment.NewLine, LoggingLabels.ProcessIdLabel, processId, Environment.NewLine));
            }

            return DiagnosticsStorage.AppendTextAsync(
                GetFilePathFromDate(DateTime.Today),
                string.Format(LoggingLabels.LocalStorageStartTimeTrackLabel, DateTime.UtcNow, key, Environment.NewLine));
        }

        public override Task StopTrackTimeAsync(string key, string? processId = null)
        {
            var ellapsedTime = TrackTimeHandler.Stop(key, processId);

            if (processId != null)
            {
                return DiagnosticsStorage.AppendTextAsync(
                    GetFilePathFromDate(DateTime.Today),
                    string.Format(LoggingLabels.LocalStorageStopTimeTrackWithProcessIdLabel, DateTime.UtcNow, key, Environment.NewLine, LoggingLabels.ProcessIdLabel, processId, Environment.NewLine, LoggingLabels.EllapsedTimeLabel, ellapsedTime, Environment.NewLine));
            }

            return DiagnosticsStorage.AppendTextAsync(
                GetFilePathFromDate(DateTime.Today),
                string.Format(LoggingLabels.LocalStorageStopTimeTrackLabel, DateTime.UtcNow, key, Environment.NewLine, LoggingLabels.EllapsedTimeLabel, ellapsedTime, Environment.NewLine));
        }
    }
}
