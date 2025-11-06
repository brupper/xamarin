using Brupper.Diagnostics;
using Brupper.Maui.Diagnostics;
using Brupper.Maui.Services;
using Brupper.Maui.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;

namespace Brupper;

/// <summary>
/// Service registration extensions for Brupper MAUI services
/// </summary>
public static class ServiceRegister
{
    /// <summary>
    /// Registers Brupper MAUI services with the dependency injection container
    /// </summary>
    /// <param name="services">The service collection to register services with</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddBrupperMaui(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Register core navigation service
        services.AddSingleton<INavigationService, NavigationService>();

        // Register platform services (these are cross-platform abstractions)
        services.AddSingleton<IConnectivity, ConnectivityService>();
        services.AddSingleton<IFileSystem, FileSystemService>();

        // Register platform-specific services
        // Note: Platform-specific implementations are registered via partial methods or platform-specific code
#if ANDROID
        services.AddSingleton<IPlatformInformationService, Platforms.Android.Services.PlatformInformationService>();
        services.AddSingleton<IPermissionHelper, Platforms.Android.Services.PermissionHelper>();
        services.AddSingleton<IImageResizer, Platforms.Android.Services.ImageResizer>();
        services.AddSingleton<IOutputRendererServices, Platforms.Android.Services.Rendering.OutputRendererServices>();
#elif IOS
        services.AddSingleton<IPlatformInformationService, Platforms.Ios.Services.PlatformInformationService>();
        services.AddSingleton<IPermissionHelper, Platforms.Ios.Services.PermissionHelper>();
        services.AddSingleton<IImageResizer, Platforms.Ios.Services.ImageResizer>();
        services.AddSingleton<IOutputRendererServices, Platforms.Ios.Services.Rendering.OutputRendererServices>();
#elif WINDOWS
        services.AddSingleton<IPlatformInformationService, Platforms.Windows.Services.PlatformInformationService>();
        services.AddSingleton<IPermissionHelper, Platforms.Windows.Services.PermissionHelper>();
        services.AddSingleton<IImageResizer, Platforms.Windows.Services.ImageResizer>();
        services.AddSingleton<IOutputRendererServices, Platforms.Windows.Services.Rendering.OutputRendererServices>();
#endif

        // Register localization service
        services.AddSingleton<ILocalizationService, LocalizationService>();

        // Register diagnostics services
        services.AddSingleton<IDiagnosticsPlatformInformationProvider>(sp => 
            sp.GetRequiredService<IPlatformInformationService>());
        services.AddSingleton<IDiagnosticsStorage, FormsStorage>();
        services.AddSingleton<FormsLogger>();

        // Register and configure Brupper Logger
        services.AddSingleton<ILogger>(sp =>
        {
            var formsLogger = sp.GetRequiredService<FormsLogger>();
            Logger.Init<FormsLogger>(formsLogger);
            Logger.Current.RegisterProvider<AppCenterLogProvider>(LogTagLevels.Medium);
            return Logger.Current;
        });

        // Register Microsoft.Extensions.Logging integration
        services.AddLogging(builder =>
        {
            builder.AddDebug();
#if DEBUG
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
#else
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
#endif
        });

        return services;
    }

    /// <summary>
    /// Configures Brupper MAUI services with custom options
    /// </summary>
    /// <typeparam name="TOptions">The options type to configure</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Configuration action for Brupper options</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddBrupperMaui<TOptions>(
        this IServiceCollection services,
        Action<TOptions> configureOptions) where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.AddBrupperMaui();
        services.Configure(configureOptions);

        return services;
    }

    /// <summary>
    /// Registers a ViewModel as a transient service.
    /// Use this for ViewModels that should be created fresh for each navigation.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to register.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddViewModel<TViewModel>(this IServiceCollection services)
        where TViewModel : class
    {
        return services.AddTransient<TViewModel>();
    }

    /// <summary>
    /// Registers a ViewModel as a singleton service.
    /// Use this for ViewModels that should persist across navigation (e.g., app-level state).
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to register.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddViewModelSingleton<TViewModel>(this IServiceCollection services)
        where TViewModel : class
    {
        return services.AddSingleton<TViewModel>();
    }

    /// <summary>
    /// Registers a View (Page or Popup) as a transient service.
    /// Views are typically transient so each navigation creates a fresh instance.
    /// </summary>
    /// <typeparam name="TView">The View type to register.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddView<TView>(this IServiceCollection services)
        where TView : class
    {
        return services.AddTransient<TView>();
    }

    /// <summary>
    /// Registers a View and its associated ViewModel together as transient services.
    /// This is a convenience method for registering a page and its ViewModel in one call.
    /// </summary>
    /// <typeparam name="TView">The View type to register.</typeparam>
    /// <typeparam name="TViewModel">The ViewModel type to register.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddViewWithViewModel<TView, TViewModel>(this IServiceCollection services)
        where TView : class
        where TViewModel : class
    {
        services.AddTransient<TView>();
        services.AddTransient<TViewModel>();
        return services;
    }
}

/// <summary>
/// Extension methods for MauiAppBuilder to configure Brupper MAUI
/// </summary>
public static class MauiProgramExtensions
{
    /// <summary>
    /// Configures the MAUI application to use Brupper MAUI services and handlers
    /// </summary>
    /// <param name="builder">The MAUI app builder</param>
    /// <returns>The builder for method chaining</returns>
    public static MauiAppBuilder UseBrupperMaui(this MauiAppBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Configure MAUI handlers
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddBrupperUIHandlers();
        });

        // Register Brupper services
        builder.Services.AddBrupperMaui();

        return builder;
    }

    /// <summary>
    /// Adds Brupper UI handlers to the MAUI handlers collection
    /// </summary>
    /// <param name="collection">The handlers collection</param>
    /// <returns>The handlers collection for method chaining</returns>
    public static IMauiHandlersCollection AddBrupperUIHandlers(this IMauiHandlersCollection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        return collection;
        //.AddInputKitHandlers()
        //.AddHandler(typeof(Button), typeof(StatefulButtonHandler))
        //.AddHandler(typeof(StatefulContentView), typeof(StatefulContentViewHandler))
        //.AddHandler(typeof(AutoCompleteView), typeof(AutoCompleteViewHandler))
        //.AddPlainer();
    }
}
