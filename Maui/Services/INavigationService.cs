using CommunityToolkit.Maui.Views;

namespace Brupper.Maui.Services;

/// <summary>
/// Navigation service interface for MAUI Shell navigation and popup management.
/// Replaces MvvmCross IMvxNavigationService patterns.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Performs navigation to one of two pages when the app is launched.
    /// </summary>
    /// <returns>A task that represents the asynchronous initialize operation.</returns>
    Task InitializeAsync();

    /// <summary>
    /// Performs hierarchical navigation to a specified page using a registered
    /// navigation route. Can optionally pass named route parameters to use for 
    /// processing on the destination page.
    /// </summary>
    /// <param name="route">The registered route to navigate to.</param>
    /// <param name="routeParameters">Optional route parameters to pass to the destination page.</param>
    /// <returns>A task that represents the asynchronous navigate operation.</returns>
    Task NavigateToAsync(string route, IDictionary<string, object>? routeParameters = null);

    /// <summary>
    /// Navigates to a page with type-safe parameters.
    /// </summary>
    /// <typeparam name="TPage">The type of page to navigate to. Must have a registered route.</typeparam>
    /// <param name="parameters">Optional parameters to pass to the page.</param>
    /// <returns>A task that represents the asynchronous navigate operation.</returns>
    Task NavigateToAsync<TPage>(IDictionary<string, object>? parameters = null) where TPage : Page;

    /// <summary>
    /// Removes the current page from the navigation stack.
    /// </summary>
    /// <returns>A task that represents the asynchronous pop operation.</returns>
    Task PopAsync();

    /// <summary>
    /// Shows a popup asynchronously.
    /// </summary>
    /// <param name="popup">The popup to show.</param>
    /// <returns>A task that represents the asynchronous show operation, containing the popup result.</returns>
    Task<object?> ShowPopupAsync(Popup popup);

    /// <summary>
    /// Shows a popup with a ViewModel.
    /// </summary>
    /// <typeparam name="TPopup">The type of popup to show.</typeparam>
    /// <typeparam name="TViewModel">The type of ViewModel for the popup.</typeparam>
    /// <param name="viewModel">The ViewModel instance to bind to the popup.</param>
    /// <returns>A task that represents the asynchronous show operation, containing the popup result.</returns>
    Task<object?> ShowPopupAsync<TPopup, TViewModel>(TViewModel viewModel)
        where TPopup : Popup, new()
        where TViewModel : class;

    /// <summary>
    /// Closes a popup with an optional result.
    /// </summary>
    /// <param name="popup">The popup to close.</param>
    /// <param name="result">Optional result to return from the popup.</param>
    void ClosePopup(Popup popup, object? result = null);

    /// <summary>
    /// Navigates to the root page.
    /// </summary>
    /// <returns>A task that represents the asynchronous navigate operation.</returns>
    Task NavigateToRootAsync();
}
