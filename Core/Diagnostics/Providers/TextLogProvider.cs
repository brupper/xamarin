using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    public abstract class TextLogProvider : ILogProvider
    {
        public IDiagnosticsStorage DiagnosticsStorage { get; set; }

        public abstract Task TrackTraceAsync(LogMessage message);

        public abstract Task TrackEventAsync(LogMessage message);

        public abstract Task StartTrackPageViewAsync(string pageName, IEnumerable<KeyValuePair<string, string>>? metaData = null);

        public abstract Task StopTrackPageViewAsync(string pageName);

        public abstract Task StartTrackTimeAsync(string key, string? processId = null);

        public abstract Task StopTrackTimeAsync(string key, string? processId = null);

        public abstract Task TrackExceptionAsync(LogMessage message);

        protected string PrepareStringMessage(LogMessage message)
        {
            var loggedMessage = new StringBuilder(message.Message);

            if (message.Tags.Any())
            {
                var tags = string.Format(
                    CultureInfo.InvariantCulture,
                    "[{0}] ",
                    string.Join(",", message.Tags));

                loggedMessage.Insert(0, tags);
            }

            if (!string.IsNullOrEmpty(message.SourceFilePath))
            {
                loggedMessage.AppendFormat(" ({0}, {1}:{2})", message.SourceFilePath, message.MemberName, message.SourceLineNumber);
            }

            if (message.MetaData.Any())
            {
                loggedMessage.AppendLine();

                foreach (var metaData in message.MetaData)
                {
                    loggedMessage.AppendFormat("  {0} : {1}", metaData.Key, metaData.Value);
                    loggedMessage.AppendLine();
                }

                loggedMessage.Remove(loggedMessage.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            }

            if (message.LogException != null && message.LogException.StackTrace != null)
            {
                loggedMessage.AppendLine();

                var stackTrace = (from line in message.LogException.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                  select string.Format(CultureInfo.InvariantCulture, "    {0}", line)).ToList();

                foreach (var line in stackTrace)
                {
                    loggedMessage.AppendLine(line);
                }

                loggedMessage.Remove(loggedMessage.Length - Environment.NewLine.Length, Environment.NewLine.Length);

                if (message.LogException.InnerException != null)
                {
                    var innerException = message.LogException.InnerException;
                    loggedMessage.AppendLine(innerException.Message);

                    if (innerException.StackTrace != null)
                    {
                        var innerStackTrace = from line in innerException.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                              select string.Format(CultureInfo.InvariantCulture, "    {0}", line);

                        foreach (var line in innerStackTrace)
                        {
                            loggedMessage.AppendLine(line);
                        }
                    }
                }
            }

            return loggedMessage.ToString();
        }
    }
}
