using Brupper.Diagnostics;
using Brupper.Maui.Diagnostics;
using Brupper.Maui.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

namespace Brupper;

/*

using MvvmCross.IoC;

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

        //ioCProvider.LazyConstructAndRegisterSingleton<IConnectivity, ConnectivityService>();
        //ioCProvider.LazyConstructAndRegisterSingleton<IFileSystem, FileSystemService>();

        ioCProvider.RegisterSingleton<IDiagnosticsPlatformInformationProvider>(ioCProvider.Resolve<IPlatformInformationService>());
        ioCProvider.ConstructAndRegisterSingleton<IDiagnosticsStorage, FormsStorage>();
        Logger.Init<FormsLogger>(ioCProvider.IoCConstruct<FormsLogger>());
        Logger.Current.RegisterProvider<AppCenterLogProvider>(LogTagLevels.Medium);
        ioCProvider.RegisterSingleton<ILogger>(() => Logger.Current);
    }
}
*/

public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseBrupperMaui(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddBrupperUIHandlers();
        });

        var services = builder.Services;

        services.AddSingleton<INavigationService, MauiNavigationService>();

        // services.AddSingleton<IConnectivity, ConnectivityService>();
        // services.AddSingleton<IFileSystem, FileSystemService>();

        services.AddSingleton<IDiagnosticsPlatformInformationProvider>(_ => _.GetService<IPlatformInformationService>());
        services.AddSingleton<IDiagnosticsStorage, FormsStorage>();
        services.AddSingleton<FormsLogger>();
        services.AddSingleton<ILogger, FormsLogger>();

        services.AddSingleton<ILogger>(_ =>
        {
            Logger.Init<FormsLogger>(_.GetService<FormsLogger>());
            Logger.Current.RegisterProvider<AppCenterLogProvider>(LogTagLevels.Medium);
            return Logger.Current;
        });

        return builder;
    }

    public static IMauiHandlersCollection AddBrupperUIHandlers(this IMauiHandlersCollection collection)
    {
        return collection;
        //.AddInputKitHandlers()
        //.AddHandler(typeof(Button), typeof(StatefulButtonHandler))
        //.AddHandler(typeof(StatefulContentView), typeof(StatefulContentViewHandler))
        //.AddHandler(typeof(AutoCompleteView), typeof(AutoCompleteViewHandler))
        //.AddPlainer();
    }
}
