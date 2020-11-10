using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    public class ConsoleLogProvider : TextLogProvider
    {
        public override Task TrackTraceAsync(LogMessage message)
        {
            Debug.WriteLine(LoggingLabels.ConsoleTraceLabel, message.Tag, PrepareStringMessage(message));

            return Task.FromResult(false);
        }

        public override Task TrackEventAsync(LogMessage message)
        {
            Debug.WriteLine(string.Format(LoggingLabels.ConsoleEventLabel, message.EventName));

            return Task.FromResult(false);
        }

        public override Task StartTrackPageViewAsync(string pageName, IEnumerable<KeyValuePair<string, string>> metaData = null)
        {
            TrackTimeHandler.Start(pageName);

            Debug.WriteLine(string.Format(LoggingLabels.ConsoleStartPageViewLabel, pageName));

            return Task.FromResult(false);
        }

        public override Task StopTrackPageViewAsync(string pageName)
        {
            var ellapsedTime = TrackTimeHandler.Stop(pageName);

            Debug.WriteLine(LoggingLabels.ConsoleStopPageViewLabel, pageName, ellapsedTime);

            return Task.FromResult(false);
        }

        public override Task TrackExceptionAsync(LogMessage message)
        {
            Debug.WriteLine(string.Format(LoggingLabels.ConsoleTrackExceptionLabel, PrepareStringMessage(message)));

            return Task.FromResult(false);
        }

        public override Task StartTrackTimeAsync(string key, string processId = null)
        {
            TrackTimeHandler.Start(key, processId);

            Debug.WriteLine(string.Format(LoggingLabels.ConsoleStartTimeTrackLabel, key));

            if (processId != null)
            {
                Debug.WriteLine(LoggingLabels.SimpleLogDetailLabel, LoggingLabels.ProcessIdLabel, processId);
            }

            return Task.FromResult(false);
        }

        public override Task StopTrackTimeAsync(string key, string processId = null)
        {
            var ellapsedTime = TrackTimeHandler.Stop(key, processId);

            Debug.WriteLine(string.Format(LoggingLabels.ConsoleStopTimeTrackLabel, key));

            if (processId != null)
            {
                Debug.WriteLine(LoggingLabels.SimpleLogDetailLabel, LoggingLabels.ProcessIdLabel, processId);
            }

            Debug.WriteLine(LoggingLabels.SimpleLogDetailLabel, LoggingLabels.EllapsedTimeLabel, ellapsedTime);

            return Task.FromResult(false);
        }
    }
}
