using Brupper.Maui.Models;
using Brupper.Maui.Services.Implementations;
using Brupper.Maui.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.Maui.Platforms.Windows.Services;

public class PlatformInformationService : BasePlatformInformationService, IPlatformInformationService
{
    public override string GetDeviceId()
    {
        return string.Empty;
    }

    public override string GetVersion()
    {
        return string.Empty;
    }

    public override string GetPlatform() => "Windows";

    public override string GetPlatformVersion() => string.Empty;

    public override string GetMachineName() => string.Empty;

    public override string GetCarrier() => string.Empty;

    public override string GetOSVersion() => string.Empty;


    public override async Task<DetailedDeviceInformation> GetDeviceInformationAsync()
    {
        var devInfo = new DetailedDeviceInformation();

        return devInfo;
    }

    private class StorageInfo
    {
        public long TotalSpace { get; set; }
        public long FreeSpace { get; set; }
        public long AvailableSpace { get; set; }
    }
}