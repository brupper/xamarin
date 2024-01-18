namespace Brupper.Maui.Services;

public interface IApplicationStateListener
{
    event EventHandler<ApplicationStateChangedEventArgs> ApplicationStateChanged;
}

public class ApplicationStateChangedEventArgs : EventArgs
{
    public ApplicationStateChangedEventArgs(ApplicationState applicationState)
    {
        ApplicationState = applicationState;
    }

    public ApplicationState ApplicationState { get; private set; }
}

public enum ApplicationState
{
    Foreground,
    Background
}
