using Brupper.Forms.Models;
using Brupper.Forms.Services.Interfaces;
using System.Threading.Tasks;

namespace Brupper.Forms.Services.Implementations
{
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
}
