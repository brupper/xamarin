using Brupper.Diagnostics;
using Brupper.IO;
using System;
using System.IO;

namespace Brupper.Maui.Diagnostics
{
    public class DefaultPlatformInformationProvider : IDiagnosticsPlatformInformationProvider
    {
        public string GetMachineName() => Environment.MachineName;

        public string GetOSVersion() => Environment.OSVersion.ToString();

        public string GetPlatform() => "<?>";

        public string GetVersion() => Environment.Version.ToString();
    }
}
