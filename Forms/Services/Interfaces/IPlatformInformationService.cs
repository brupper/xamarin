namespace Brupper.Forms.Services.Interfaces
{
    public interface IPlatformInformationService
    {
        string GetDeviceId();

        string GetVersion();

        string GetPlatform();

        string GetPlatformVersion();

        string GetMachineName();

        string GetCarrier();

        string GetOSVersion();
    }
}
