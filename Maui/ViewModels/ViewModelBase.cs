using Brupper.Maui.Services;
using Brupper.Maui.ViewModels;
using Brupper.ViewModels.Popups;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace Brupper.Maui.ViewModels;

/// <summary>
/// Base ViewModel for all MAUI ViewModels.
/// Replaces MvvmCross MvxViewModel with MVVM Community Toolkit ObservableObject.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject, IPopupDialogViewModel, ISupportBrupperViewModel
{
    #region Fields

    /// <summary>
    /// Event raised when an alert message should be displayed.
    /// </summary>
    public event EventHandler<string>? Alert;

    private int isBusyCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SyncMessage))]
    private string? message;

    [ObservableProperty]
    private string? syncMessage;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="navigationService">The navigation service.</param>
    protected ViewModelBase(ILogger logger, INavigationService navigationService)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of this ViewModel (for logging/tracking).
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the version string from AppInfo.
    /// </summary>
    public string Version => $"{AppInfo.VersionString} ({AppInfo.BuildString})";

    /// <summary>
    /// Gets the logger instance.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the navigation service.
    /// </summary>
    protected INavigationService NavigationService { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the view can be destroyed.
    /// </summary>
    public virtual bool CanViewDestroy { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the ViewModel is busy.
    /// Uses a counter to handle multiple concurrent busy operations.
    /// </summary>
    public bool IsBusy
    {
        get => isBusyCount > 0;
        set
        {
            var newCount = isBusyCount + (value ? 1 : -1);
            if (SetProperty(ref isBusyCount, newCount))
            {
                OnPropertyChanged(nameof(IsBusy));
            }
        }
    }

    /// <summary>
    /// Gets the BackPressedCommand (implements IPopupDialogViewModel).
    /// </summary>
    ICommand IPopupDialogViewModel.BackPressedCommand => BackPressedCommand;

    #endregion

    #region Lifecycle Methods

    /// <summary>
    /// Called when the ViewModel is being initialized.
    /// Override this method to perform initialization logic.
    /// </summary>
    public virtual Task InitializeAsync()
    {
        Logger.LogInformation("Initializing: {ViewModelName}", Name);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the view is appearing.
    /// </summary>
    public virtual void ViewAppearing()
    {
        Logger.LogDebug("{ViewModelName} ViewAppearing", Name);
    }

    /// <summary>
    /// Called when the view has appeared.
    /// </summary>
    public virtual void ViewAppeared()
    {
        Logger.LogDebug("{ViewModelName} ViewAppeared", Name);
    }

    /// <summary>
    /// Called when the view is disappearing.
    /// </summary>
    public virtual void ViewDisappearing()
    {
        Logger.LogDebug("{ViewModelName} ViewDisappearing", Name);
    }

    /// <summary>
    /// Called when the view has disappeared.
    /// </summary>
    public virtual void ViewDisappeared()
    {
        Logger.LogDebug("{ViewModelName} ViewDisappeared", Name);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Invokes an action on the main thread asynchronously.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <param name="maskExceptions">Whether to mask exceptions (true) or propagate them (false).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task InvokeOnMainThreadAsync(Task action, bool maskExceptions = true)
    {
        var tcs = new TaskCompletionSource<bool>();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await action;
                tcs.TrySetResult(true);
            }
            catch (Exception e)
            {
                if (!maskExceptions)
                {
                    tcs.TrySetException(e);
                }
                else
                {
                    Logger.LogError(e, "Error in InvokeOnMainThreadAsync");
                    tcs.TrySetResult(false);
                }
            }
        });

        await tcs.Task;
    }

    /// <summary>
    /// Command executed when the back button is pressed.
    /// Default implementation navigates back.
    /// </summary>
    [RelayCommand]
    protected virtual async Task BackPressedAsync()
    {
        await NavigationService.PopAsync();
    }

    /// <summary>
    /// Raises an alert event with the specified message.
    /// </summary>
    /// <param name="message">The alert message.</param>
    protected virtual void OnAlert(string message)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() => Alert?.Invoke(this, message));
        }
        catch (PlatformNotSupportedException)
        {
            Logger.LogWarning("Platform does not support MainThread operations");
        }
    }

    /// <summary>
    /// Checks if internet connectivity is available.
    /// </summary>
    /// <param name="showAlert">Whether to show an alert if no internet is available.</param>
    /// <returns>True if internet is available; otherwise, false.</returns>
    public bool CheckInternet(bool showAlert = true)
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            if (showAlert)
            {
                // TODO: Implement localized message
                OnAlert("No internet connection available");
            }
            return false;
        }

        return true;
    }

    #endregion
}
