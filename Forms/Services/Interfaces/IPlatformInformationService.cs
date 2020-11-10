using Brupper.Forms.Models;
using System.Threading.Tasks;

namespace Brupper.Forms.Services.Interfaces
{
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
}
