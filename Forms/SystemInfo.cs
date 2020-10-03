using Brupper.Forms.Services.Interfaces;
using Microsoft.AppCenter.Crashes;
using MvvmCross;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Brupper.Forms
{
    public class SystemInfo
    {
        public IPlatformInformationService PlatformInformation
        {
            get
            {
                Mvx.IoCProvider.TryResolve<IPlatformInformationService>(out var reference);
                return reference;
            }
        }

        #region Platform informations

        public string DeviceId => PlatformInformation?.GetDeviceId();

        public string Version => PlatformInformation?.GetVersion() ?? AppInfo.VersionString;

        public string Platform => PlatformInformation?.GetPlatform() ?? DeviceInfo.Platform.ToString();

        public string PlatformVersion => PlatformInformation?.GetPlatformVersion() ?? DeviceInfo.Version.ToString();

        public string MachineName => PlatformInformation?.GetMachineName();

        public string Carrier => PlatformInformation?.GetCarrier();

        public string OSVersion => PlatformInformation?.GetOSVersion();

        #endregion

        public Location GeoPosition { get; set; }

        public string BatteryInfo { get; set; }

        public string ConnectivityInfo { get; set; }

        public Task InitializeAsync()
        {
            var task = InitializeGeolocationAsync();

            var task2 = InitializeBatteryInfoAsync();
            var task3 = InitializeConnectivityInfoAsync();

            return Task.WhenAll(task, task2, task3);
        }

        /// <summary> https://docs.microsoft.com/en-us/xamarin/essentials/geolocation </summary>
        public async Task InitializeGeolocationAsync()
        {
            try
            {
                //var request = new GeolocationRequest(GeolocationAccuracy.High);
                //var location = await Geolocation.GetLocationAsync(request);
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    GeoPosition = location;
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Crashes.TrackError(fnsEx);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Crashes.TrackError(fneEx);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Crashes.TrackError(pEx);
            }
            catch (Exception ex)
            {
                // Unable to get location
                Crashes.TrackError(ex);
            }
        }

        /// <summary> https://docs.microsoft.com/en-us/xamarin/essentials/battery </summary>
        public Task InitializeBatteryInfoAsync()
        {
            return Task.Run(() =>
            {
                // Register for battery changes, be sure to unsubscribe when needed
                Battery.BatteryInfoChanged += (sender, e) =>
                {
                    //var level = e.ChargeLevel;
                    //var state = e.State;
                    //var source = e.PowerSource;
                    //Console.WriteLine($"Reading: Level: {level}, State: {state}, Source: {source}");
                };

                var level = Battery.ChargeLevel; // returns 0.0 to 1.0 or 1.0 when on AC or no battery.
                BatteryInfo = $"{level * 100}%";
                var state = Battery.State;

                switch (state)
                {
                    case BatteryState.Charging:
                        // Currently charging
                        break;
                    case BatteryState.Full:
                        // Battery is full
                        break;
                    case BatteryState.Discharging:
                    case BatteryState.NotCharging:
                        // Currently discharging battery or not being charged
                        break;
                    case BatteryState.NotPresent:
                    // Battery doesn't exist in device (desktop computer)
                    case BatteryState.Unknown:
                        // Unable to detect battery state
                        break;
                }

                var source = Battery.PowerSource;

                switch (source)
                {
                    case BatteryPowerSource.Battery:
                        // Being powered by the battery
                        break;
                    case BatteryPowerSource.AC:
                        // Being powered by A/C unit
                        break;
                    case BatteryPowerSource.Usb:
                        // Being powered by USB cable
                        break;
                    case BatteryPowerSource.Wireless:
                        // Powered via wireless charging
                        break;
                    case BatteryPowerSource.Unknown:
                        // Unable to detect power source
                        break;
                }
            });
        }

        /// <summary> https://docs.microsoft.com/en-us/xamarin/essentials/connectivity </summary>
        public Task InitializeConnectivityInfoAsync()
        {
            return Task.Run(() =>
            {
                var current = Connectivity.NetworkAccess;

                // if (current != NetworkAccess.Unknown && current != NetworkAccess.None)
                if (current != NetworkAccess.Internet)
                {
                    // Connection to internet is available
                }

                ConnectivityInfo = current.ToString();
            });
        }

        public override string ToString()
        {
            return $"Battery: {BatteryInfo} Network: {ConnectivityInfo} Lat: {GeoPosition?.Latitude} Lon: {GeoPosition?.Longitude}";
        }
    }
}
