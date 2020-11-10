using Brupper.Diagnostics;
using Brupper.Forms.Diagnostics;
using Brupper.Forms.Services;
using Brupper.Forms.Services.Interfaces;
using MvvmCross.IoC;
using System;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Forms
{
    /// <summary> . </summary>
    public static class ServiceRegister
    {
        /// <summary> . </summary>
        /// <exception cref="ArgumentNullException"><paramref name="ioCProvider"/> is <c>null</c>.</exception>
        public static void RegisterCrossServices(this IMvxIoCProvider ioCProvider)
        {
            if (ioCProvider == null)
            {
                throw new ArgumentNullException("IMvxIoCProvider is not initialized yet!");
            }

            ioCProvider.LazyConstructAndRegisterSingleton<IConnectivity, ConnectivityService>();
            ioCProvider.LazyConstructAndRegisterSingleton<IFileSystem, FileSystemService>();

            ioCProvider.RegisterSingleton<IDiagnosticsPlatformInformationProvider>(ioCProvider.Resolve<IPlatformInformationService>());
            ioCProvider.ConstructAndRegisterSingleton<IDiagnosticsStorage, FormsStorage>();
            Logger.Init<FormsLogger>(ioCProvider.IoCConstruct<FormsLogger>());
            Logger.Current.RegisterProvider<AppCenterLogProvider>(LogTagLevels.Medium);
            ioCProvider.RegisterSingleton<ILogger>(() => Logger.Current);
        }
    }
}
