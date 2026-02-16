using Brupper.Maui.Models;
using System.Threading.Tasks;

namespace Brupper.Maui.Services;

public interface IPlatformInformationService : Brupper.Diagnostics.IDiagnosticsPlatformInformationProvider
{
    string GetDeviceId();

    string GetVersion();

    string GetPlatform();

    string GetPlatformVersion();

    string GetMachineName();

    string GetCarrier();

    string GetOSVersion();

    Task<DetailedDeviceInformation> GetDeviceInformationAsync();
}
