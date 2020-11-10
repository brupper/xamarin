// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    /// <summary> Represents device and app specific system information. </summary>
    public class SystemInfo
    {
        public SystemInfo(
            string operatingSystem,
            string machineName,
            string appVersion,
            string server)
        {
            OperatingSystem = operatingSystem;
            MachineName = machineName;
            AppVersion = appVersion;
            Server = server;
        }

        public string OperatingSystem { get; }

        public string MachineName { get; }

        public string AppVersion { get; }

        public string Server { get; }
    }
}
