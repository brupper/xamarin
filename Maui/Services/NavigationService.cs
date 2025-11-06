using CommunityToolkit.Maui.Views;
using System.Linq;

namespace Brupper.Maui.Services;

/// <summary>
/// Shell-based navigation service for MAUI.
/// Replaces MvvmCross IMvxNavigationService with MAUI Shell navigation patterns.
/// 
/// Routes must be registered using RouteRegistration.RegisterRoutes() or Routing.RegisterRoute()
/// before navigation. See ROUTING_MIGRATION.md for migration guide from MvvmCross presentation attributes.
/// </summary>
public class NavigationService : INavigationService
{
    /// <summary>
    /// Performs navigation to one of two pages when the app is launched.
    /// </summary>
    /// <returns>A task that represents the asynchronous initialize operation.</returns>
    public Task InitializeAsync()
    {
        // Initialize app navigation - can be overridden in derived classes
        return Task.CompletedTask;
    }

    /// <summary>
    /// Performs hierarchical navigation to a specified page using a registered
    /// navigation route. Can optionally pass named route parameters to use for 
    /// processing on the destination page.
    /// </summary>
    /// <param name="route">The registered route to navigate to.</param>
    /// <param name="routeParameters">Optional route parameters to pass to the destination page.</param>
    /// <returns>A task that represents the asynchronous navigate operation.</returns>
    public async Task NavigateToAsync(string route, IDictionary<string, object>? routeParameters = null)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            throw new ArgumentNullException(nameof(route));
        }

        if (Shell.Current == null)
        {
            throw new InvalidOperationException("Shell.Current is null. Ensure the app uses Shell navigation.");
        }

        if (routeParameters != null && routeParameters.Any())
        {
            await Shell.Current.GoToAsync(route, routeParameters);
        }
        else
        {
            await Shell.Current.GoToAsync(route);
        }
    }

    /// <summary>
    /// Removes the current page from the navigation stack.
    /// </summary>
    /// <returns>A task that represents the asynchronous pop operation.</returns>
    public async Task PopAsync()
    {
        if (Shell.Current == null)
        {
            throw new InvalidOperationException("Shell.Current is null. Ensure the app uses Shell navigation.");
        }

        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Navigates to a page with type-safe parameters.
    /// </summary>
    /// <typeparam name="TPage">The type of page to navigate to. Must have a Route attribute.</typeparam>
    /// <param name="parameters">Optional parameters to pass to the page.</param>
    /// <returns>A task that represents the asynchronous navigate operation.</returns>
    public Task NavigateToAsync<TPage>(IDictionary<string, object>? parameters = null) where TPage : Page
    {
        var route = GetRouteForPage<TPage>();
        return NavigateToAsync(route, parameters);
    }

    /// <summary>
    /// Shows a popup asynchronously.
    /// </summary>
    /// <param name="popup">The popup to show.</param>
    /// <returns>A task that represents the asynchronous show operation, containing the popup result.</returns>
    public async Task<object?> ShowPopupAsync(Popup popup)
    {
        if (popup == null)
        {
            throw new ArgumentNullException(nameof(popup));
        }

        if (Shell.Current?.CurrentPage == null)
        {
            throw new InvalidOperationException("Shell.Current.CurrentPage is null. Cannot show popup.");
        }

        return await Shell.Current.CurrentPage.ShowPopupAsync(popup);
    }

    /// <summary>
    /// Shows a popup with a ViewModel.
    /// </summary>
    /// <typeparam name="TPopup">The type of popup to show.</typeparam>
    /// <typeparam name="TViewModel">The type of ViewModel for the popup.</typeparam>
    /// <param name="viewModel">The ViewModel instance to bind to the popup.</param>
    /// <returns>A task that represents the asynchronous show operation, containing the popup result.</returns>
    public async Task<object?> ShowPopupAsync<TPopup, TViewModel>(TViewModel viewModel)
        where TPopup : Popup, new()
        where TViewModel : class
    {
        var popup = new TPopup
        {
            BindingContext = viewModel
        };

        return await ShowPopupAsync(popup);
    }

    /// <summary>
    /// Closes a popup with an optional result.
    /// </summary>
    /// <param name="popup">The popup to close.</param>
    /// <param name="result">Optional result to return from the popup.</param>
    public void ClosePopup(Popup popup, object? result = null)
    {
        if (popup == null)
        {
            throw new ArgumentNullException(nameof(popup));
        }

        popup.Close(result);
    }

    /// <summary>
    /// Navigates to the root page.
    /// </summary>
    /// <returns>A task that represents the asynchronous navigate operation.</returns>
    public async Task NavigateToRootAsync()
    {
        if (Shell.Current == null)
        {
            throw new InvalidOperationException("Shell.Current is null. Ensure the app uses Shell navigation.");
        }

        await Shell.Current.GoToAsync("//");
    }

    /// <summary>
    /// <summary>
    /// Gets the registered route for a page type.
    /// Falls back to using the type name if no route is explicitly registered.
    /// </summary>
    /// <typeparam name="TPage">The page type.</typeparam>
    /// <returns>The route string for the page.</returns>
    private static string GetRouteForPage<TPage>() where TPage : Page
    {
        var pageType = typeof(TPage);
        
        // MAUI doesn't provide a way to query registered routes,
        // so we use the type name as the route.
        // Routes must be registered explicitly using Routing.RegisterRoute()
        return pageType.Name;
    }
}
