using Brupper.Forms.Services.Implementations;
using Brupper.Forms.Services.Interfaces;
using CoreTelephony;
using Foundation;
using System;
using UIKit;

namespace Brupper.Forms.Platforms.iOS.Services
{
    public class PlatformInformationService : BasePlatformInformationService, IPlatformInformationService
    {
        public override string GetDeviceId() => FormattableString.Invariant($"{UIDevice.CurrentDevice.IdentifierForVendor}");

        public override string GetVersion() => NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();

        public override string GetPlatform() => FormattableString.Invariant($"{UIDevice.CurrentDevice.SystemName}, {UIDevice.CurrentDevice.Model}");

        public override string GetPlatformVersion() => UIDevice.CurrentDevice.SystemVersion;

        public override string GetMachineName() => UIDevice.CurrentDevice.Name;

        public override string GetCarrier()
        {
            using (var networkInfo = new CTTelephonyNetworkInfo())
            {
                if (networkInfo.SubscriberCellularProvider != null)
                {
                    return networkInfo.SubscriberCellularProvider.CarrierName;
                }
                else
                {
                    return "Not connected";
                }
            }
        }

        public override string GetOSVersion() => NSProcessInfo.ProcessInfo.OperatingSystemVersionString;
    }
}