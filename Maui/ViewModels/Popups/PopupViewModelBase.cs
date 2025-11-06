using Brupper.Maui.Services;
using Brupper.ViewModels.Popups;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace Brupper.Maui.ViewModels.Popups;

/// <summary>
/// Base ViewModel for popup views.
/// Replaces MvvmCross MvxPopupViewModel with MAUI-compatible patterns.
/// </summary>
public abstract partial class PopupViewModelBase : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopupViewModelBase"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="navigationService">The navigation service.</param>
    protected PopupViewModelBase(ILogger logger, INavigationService navigationService)
        : base(logger, navigationService)
    {
    }

    /// <summary>
    /// Reference to the popup that hosts this ViewModel.
    /// Set by the view when the popup is created.
    /// </summary>
    public Popup? Popup { get; set; }

    /// <summary>
    /// Command executed when the back button is pressed in a popup.
    /// Default implementation closes the popup.
    /// </summary>
    protected override async Task BackPressedAsync()
    {
        await ClosePopupAsync(null);
    }

    /// <summary>
    /// Closes the popup with an optional result.
    /// </summary>
    /// <param name="result">The result to return from the popup.</param>
    protected virtual Task ClosePopupAsync(object? result)
    {
        if (Popup != null)
        {
            NavigationService.ClosePopup(Popup, result);
        }
        else
        {
            Logger.LogWarning("Popup is null, cannot close");
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Base ViewModel for popup views with initialization parameter support.
/// </summary>
/// <typeparam name="TParam">The type of parameter passed during initialization.</typeparam>
public abstract partial class PopupViewModelBase<TParam> : PopupViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopupViewModelBase{TParam}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="navigationService">The navigation service.</param>
    protected PopupViewModelBase(ILogger logger, INavigationService navigationService)
        : base(logger, navigationService)
    {
    }

    /// <summary>
    /// Called when the popup is being initialized with a parameter.
    /// </summary>
    /// <param name="parameter">The initialization parameter.</param>
    public virtual Task InitializeAsync(TParam parameter)
    {
        Logger.LogInformation("Initializing popup {ViewModelName} with parameter of type {ParameterType}",
            Name, typeof(TParam).Name);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Base ViewModel for popup views with initialization parameter and result support.
/// </summary>
/// <typeparam name="TParam">The type of parameter passed during initialization.</typeparam>
/// <typeparam name="TResult">The type of result returned from this popup.</typeparam>
public abstract partial class PopupViewModelBase<TParam, TResult> : PopupViewModelBase
{
    private TaskCompletionSource<TResult?>? closeCompletionSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopupViewModelBase{TParam, TResult}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="navigationService">The navigation service.</param>
    protected PopupViewModelBase(ILogger logger, INavigationService navigationService)
        : base(logger, navigationService)
    {
    }

    /// <summary>
    /// Called when the popup is being initialized with a parameter.
    /// </summary>
    /// <param name="parameter">The initialization parameter.</param>
    public virtual Task InitializeAsync(TParam parameter)
    {
        Logger.LogInformation("Initializing popup {ViewModelName} with parameter of type {ParameterType}",
            Name, typeof(TParam).Name);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Closes the popup with a result.
    /// </summary>
    /// <param name="result">The result to return.</param>
    protected virtual Task ClosePopupAsync(TResult? result)
    {
        closeCompletionSource?.TrySetResult(result);

        if (Popup != null)
        {
            NavigationService.ClosePopup(Popup, result);
        }
        else
        {
            Logger.LogWarning("Popup is null, cannot close");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets a task that completes when the popup is closed, returning the result.
    /// </summary>
    /// <returns>A task that represents the result of the popup.</returns>
    public Task<TResult?> GetResultAsync()
    {
        closeCompletionSource ??= new TaskCompletionSource<TResult?>();
        return closeCompletionSource.Task;
    }
}
