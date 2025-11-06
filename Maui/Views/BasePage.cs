namespace Brupper.Maui.Views;

/// <summary>
/// Base page for all MAUI content pages.
/// Replaces MvvmCross MvxBasePage with native MAUI patterns.
/// Preserves BackPressedCommand functionality and lifecycle events from legacy implementation.
/// </summary>
/// <typeparam name="TViewModel">The type of ViewModel for this page.</typeparam>
public class BasePage<TViewModel> : ContentPage
    where TViewModel : class
{
    /// <summary>
    /// Gets or sets the ViewModel for this page.
    /// </summary>
    public TViewModel? ViewModel
    {
        get => BindingContext as TViewModel;
        set => BindingContext = value;
    }

    /// <summary>
    /// Called when the page is appearing.
    /// Invokes ViewAppearing and ViewAppeared on the ViewModel if those methods exist.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        InvokeViewModelMethod("ViewAppearing");
        InvokeViewModelMethod("ViewAppeared");
    }

    /// <summary>
    /// Called when the page is disappearing.
    /// Invokes ViewDisappearing and ViewDisappeared on the ViewModel if those methods exist.
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        InvokeViewModelMethod("ViewDisappearing");
        InvokeViewModelMethod("ViewDisappeared");
    }

    /// <summary>
    /// Handles the hardware back button press.
    /// Executes the BackPressedCommand on the ViewModel if it exists.
    /// </summary>
    /// <returns>True if the back button press was handled; otherwise, false.</returns>
    protected override bool OnBackButtonPressed()
    {
        // Check if ViewModel has a BackPressedCommand property
        if (ViewModel != null)
        {
            var viewModelType = ViewModel.GetType();
            var commandProperty = viewModelType.GetProperty("BackPressedCommand");
            
            if (commandProperty != null)
            {
                var command = commandProperty.GetValue(ViewModel) as System.Windows.Input.ICommand;
                
                if (command != null && command.CanExecute(null))
                {
                    command.Execute(null);
                    return true;
                }
            }
        }

        return base.OnBackButtonPressed();
    }

    /// <summary>
    /// Invokes a method on the ViewModel if it exists using reflection.
    /// This enables calling lifecycle methods (ViewAppearing, ViewAppeared, etc.) without requiring the ViewModel to implement a specific interface.
    /// </summary>
    /// <param name="methodName">The name of the method to invoke.</param>
    protected void InvokeViewModelMethod(string methodName)
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
