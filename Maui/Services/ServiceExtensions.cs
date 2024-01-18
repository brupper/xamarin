using Microsoft.Extensions.DependencyInjection;

namespace Brupper.Maui.Services;

public static class ServiceProvider
{
    public static TService GetService<TService>()
        => Current.GetService<TService>();

    public static IServiceProvider Current
        =>
#if WINDOWS10_0_17763_0_OR_GREATER
			MauiWinUIApplication.Current.Services;
#elif ANDROID
            Microsoft.Maui.MauiApplication.Current.Services;
#elif IOS || MACCATALYST
            Microsoft.Maui.MauiUIApplicationDelegate.Current.Services;
#else
            null;
#endif
}