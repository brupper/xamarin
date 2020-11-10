using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    public class UserGroupLoggerSection
    {
        public List<string> UserIds { get; set; }
            = new List<string>();

        public Dictionary<LogProviderName, LogLevel> UserGroupsProviders { get; set; }
            = new Dictionary<LogProviderName, LogLevel>();
    }
}
