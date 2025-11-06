using Microsoft.Extensions.Logging;

namespace Brupper.Maui.Services;

/// <summary>
/// Centralized route registration for Shell navigation.
/// Replaces MvvmCross PopupPresentationAttribute and page presentation attributes.
/// </summary>
public static class RouteRegistration
{
    private static ILogger? logger;

    /// <summary>
    /// Registers all application routes for Shell navigation.
    /// Call this from MauiProgram.cs or App.xaml.cs during startup.
    /// </summary>
    /// <param name="loggerFactory">Logger factory for logging route registration.</param>
    public static void RegisterRoutes(ILoggerFactory? loggerFactory = null)
    {
        logger = loggerFactory?.CreateLogger("RouteRegistration");

        // Example route registrations:
        // Routing.RegisterRoute("popup/alert", typeof(AlertPopup));
        // Routing.RegisterRoute("popup/question", typeof(QuestionPopup));
        // Routing.RegisterRoute("popup/information", typeof(InformationPopup));
        // Routing.RegisterRoute("page/editor", typeof(EditorPage));
        // Routing.RegisterRoute("page/selector", typeof(SelectorPage));

        logger?.LogInformation("Route registration completed");
    }

    /// <summary>
    /// Registers a route with Shell navigation.
    /// </summary>
    /// <param name="route">The route string (e.g., "popup/alert", "page/editor").</param>
    /// <param name="pageType">The page/popup type to navigate to.</param>
    public static void RegisterRoute(string route, Type pageType)
    {
        Routing.RegisterRoute(route, pageType);
        logger?.LogDebug("Registered route '{Route}' -> {PageType}", route, pageType.Name);
    }

    /// <summary>
    /// Registers multiple routes at once.
    /// </summary>
    /// <param name="routes">Dictionary of route strings to page types.</param>
    public static void RegisterRoutes(Dictionary<string, Type> routes)
    {
        foreach (var (route, pageType) in routes)
        {
            RegisterRoute(route, pageType);
        }
    }

    /// <summary>
    /// Route constants for type-safe navigation.
    /// Use these constants instead of magic strings in navigation code.
    /// </summary>
    public static class Routes
    {
        // Popup routes (replace PopupPresentationAttribute usage)
        public const string AlertPopup = "popup/alert";
        public const string QuestionPopup = "popup/question";
        public const string InformationPopup = "popup/information";
        public const string LoadingPopup = "popup/loading";

        // Page routes (replace MvxContentPagePresentationAttribute usage)
        public const string EditorPage = "page/editor";
        public const string SelectorPage = "page/selector";
        public const string ItemsPage = "page/items";

        // Add more routes as needed during migration
    }
}
