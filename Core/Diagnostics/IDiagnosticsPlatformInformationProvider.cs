namespace Brupper.Diagnostics
{
    public interface IDiagnosticsPlatformInformationProvider
    {
        string GetVersion();
        string GetMachineName();
        string GetPlatform();
        string GetOSVersion();
    }
}
