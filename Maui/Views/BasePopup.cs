using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace Brupper.Maui.Views;

/// <summary>
/// Base popup for all MAUI popups.
/// Replaces MvvmCross MvxPopupPage with CommunityToolkit.Maui.Popup patterns.
/// Preserves lifecycle events, BackPressedCommand, and CloseWhenBackgroundIsClicked functionality.
/// </summary>
/// <typeparam name="TViewModel">The type of ViewModel for this popup.</typeparam>
public class BasePopup<TViewModel> : Popup
    where TViewModel : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BasePopup{TViewModel}"/> class.
    /// </summary>
    public BasePopup()
    {
        // MAUI Popup uses CanBeDismissedByTappingOutsideOfPopup instead of CloseWhenBackgroundIsClicked
        CanBeDismissedByTappingOutsideOfPopup = false;
        
        // Subscribe to popup events for lifecycle management
        Opened += OnPopupOpened;
        Closed += OnPopupClosed;
    }

    /// <summary>
    /// Gets or sets the ViewModel for this popup.
    /// </summary>
    public TViewModel? ViewModel
    {
        get => BindingContext as TViewModel;
        set => BindingContext = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the popup can be dismissed by tapping outside.
    /// Replaces CloseWhenBackgroundIsClicked from legacy implementation.
    /// </summary>
    public bool CloseWhenBackgroundIsClicked
    {
        get => CanBeDismissedByTappingOutsideOfPopup;
        set => CanBeDismissedByTappingOutsideOfPopup = value;
    }

    /// <summary>
    /// Event handler for popup opened event.
    /// Replaces OnAppearing lifecycle event and ViewAppearing/ViewAppeared from MvvmCross.
    /// </summary>
    private void OnPopupOpened(object? sender, PopupOpenedEventArgs e)
    {
        // Call ViewModel lifecycle methods if they exist
        InvokeViewModelMethod("ViewAppearing");
        InvokeViewModelMethod("ViewAppeared");
    }

    /// <summary>
    /// Event handler for popup closed event.
    /// Replaces OnDisappearing lifecycle event and ViewDisappearing/ViewDisappeared from MvvmCross.
    /// </summary>
    private void OnPopupClosed(object? sender, PopupClosedEventArgs e)
    {
        // Check if we should execute BackPressedCommand
        if (e.WasDismissedByTappingOutsideOfPopup && CloseWhenBackgroundIsClicked && ViewModel != null)
        {
            ExecuteBackPressedCommand();
        }

        InvokeViewModelMethod("ViewDisappearing");
        InvokeViewModelMethod("ViewDisappeared");
    }

    /// <summary>
    /// Executes the BackPressedCommand on the ViewModel if it exists.
    /// </summary>
    protected void ExecuteBackPressedCommand()
    {
        if (ViewModel == null)
            return;

        var viewModelType = ViewModel.GetType();
        var commandProperty = viewModelType.GetProperty("BackPressedCommand");

        if (commandProperty != null)
        {
            var command = commandProperty.GetValue(ViewModel) as System.Windows.Input.ICommand;

            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }

    /// <summary>
    /// Invokes a method on the ViewModel if it exists using reflection.
    /// </summary>
    /// <param name="methodName">The name of the method to invoke.</param>
    private void InvokeViewModelMethod(string methodName)
    {
        if (ViewModel == null)
            return;

        var viewModelType = ViewModel.GetType();
        var method = viewModelType.GetMethod(methodName);

        if (method != null)
        {
            method.Invoke(ViewModel, null);
        }
    }
}
