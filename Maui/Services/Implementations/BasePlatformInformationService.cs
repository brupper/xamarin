using Brupper.Maui.Models;
using System.Threading.Tasks;

namespace Brupper.Maui.Services.Implementations;

public abstract class BasePlatformInformationService : IPlatformInformationService
{
    public abstract string GetDeviceId();
    public abstract string GetVersion();
    public abstract string GetPlatform();
    public abstract string GetPlatformVersion();
    public abstract string GetMachineName();
    public abstract string GetCarrier();
    public abstract string GetOSVersion();

    public abstract Task<DetailedDeviceInformation> GetDeviceInformationAsync();

    public override string ToString()
    {
        return $"Version: {GetVersion()} Platform: {GetPlatform()} #{GetPlatformVersion()} @{GetMachineName()}, {GetCarrier()}, OS:{GetOSVersion()} UUD: {GetDeviceId()}";
    }
}
