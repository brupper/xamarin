using System;
using System.Collections.Generic;
using System.Linq;

namespace Brupper.Diagnostics
{
    /// <summary> A custom log message based on exception or a message from the developer its common to all the platforms </summary>
    public class LogMessage
    {
        public LogMessage(SystemInfo systemInfo)
        {
            this.SystemInfo = systemInfo;
        }

        public SystemInfo SystemInfo { get; }

        public string CustomerAccount { get; set; }

        public string UserName { get; set; }

        public Exception LogException { get; set; }

        public string Message { get; set; }

        public string EventName { get; set; }

        public string PageName { get; set; }

        public LogTag Tag { get; set; }

        public DateTime OccurredOn { get; }
            = DateTime.UtcNow;

        public IList<string> Tags { get; }
            = new List<string>();

        public string MemberName { get; set; }

        public string SourceFilePath { get; set; }

        public int SourceLineNumber { get; set; }

        public IEnumerable<KeyValuePair<string, string>> MetaData { get; set; }
            = Enumerable.Empty<KeyValuePair<string, string>>();
    }
}
