using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Telephony;
using Brupper.Forms.Services.Implementations;
using Brupper.Forms.Services.Interfaces;
using System;

namespace Brupper.Forms.Platforms.Android.Services
{
    public class PlatformInformationService : BasePlatformInformationService, IPlatformInformationService
    {
        public override string GetDeviceId()
        {
            return Plugin.DeviceInfo.CrossDeviceInfo.Current.Id;
        }

        public override string GetVersion()
        {
            var context = Application.Context;

            PackageManager manager = context?.PackageManager;
            PackageInfo info = manager?.GetPackageInfo(context.PackageName, 0);

            return info?.VersionName;
        }

        public override string GetPlatform() => "Android";

        public override string GetPlatformVersion() => FormattableString.Invariant($"{Build.VERSION.Release}, {Build.VERSION.SdkInt}");

        public override string GetMachineName() => FormattableString.Invariant($"{Build.Manufacturer}, {Build.Model}, {Build.Product}");

        public override string GetCarrier()
        {
            var manager = (TelephonyManager)Application.Context.GetSystemService(Application.TelephonyService);
            return manager.NetworkOperatorName;
        }

        public override string GetOSVersion() => Build.VERSION.BaseOs;
    }
}