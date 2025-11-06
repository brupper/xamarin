using Brupper.Maui.Services;
using CommunityToolkit.Maui.Views;

namespace Brupper.Maui;

/// <summary>
/// Extension methods for navigation service to provide convenient navigation patterns.
/// </summary>
public static class NavigationExtensions
{
    /// <summary>
    /// Navigates to a page and passes a single parameter.
    /// </summary>
    /// <typeparam name="TPage">The type of page to navigate to.</typeparam>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="parameterName">The parameter name.</param>
    /// <param name="parameterValue">The parameter value.</param>
    /// <returns>A task that represents the asynchronous navigate operation.</returns>
    public static Task NavigateToAsync<TPage>(
        this INavigationService navigationService,
        string parameterName,
        object parameterValue) where TPage : Page
    {
        var parameters = new Dictionary<string, object>
        {
            { parameterName, parameterValue }
        };

        return navigationService.NavigateToAsync<TPage>(parameters);
    }

    /// <summary>
    /// Navigates to a page without parameters.
    /// </summary>
    /// <typeparam name="TPage">The type of page to navigate to.</typeparam>
    /// <param name="navigationService">The navigation service.</param>
    /// <returns>A task that represents the asynchronous navigate operation.</returns>
    public static Task NavigateToAsync<TPage>(this INavigationService navigationService) where TPage : Page
    {
        return navigationService.NavigateToAsync<TPage>(null);
    }

    /// <summary>
    /// Shows a popup and waits for the result.
    /// </summary>
    /// <typeparam name="TPopup">The type of popup to show.</typeparam>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="viewModel">The ViewModel to bind to the popup.</param>
    /// <returns>A task that represents the asynchronous show operation, containing the popup result.</returns>
    public static Task<object?> ShowPopupAsync<TPopup>(
        this INavigationService navigationService,
        object viewModel) where TPopup : Popup, new()
    {
        return navigationService.ShowPopupAsync<TPopup, object>(viewModel);
    }

    /// <summary>
    /// Shows a popup without a ViewModel.
    /// </summary>
    /// <typeparam name="TPopup">The type of popup to show.</typeparam>
    /// <param name="navigationService">The navigation service.</param>
    /// <returns>A task that represents the asynchronous show operation, containing the popup result.</returns>
    public static async Task<object?> ShowPopupAsync<TPopup>(this INavigationService navigationService) 
        where TPopup : Popup, new()
    {
        var popup = new TPopup();
        return await navigationService.ShowPopupAsync(popup);
    }
}

