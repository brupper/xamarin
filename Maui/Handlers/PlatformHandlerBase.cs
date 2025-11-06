using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace Brupper.Maui.Handlers;

/// <summary>
/// Abstract base class for platform handlers providing common implementation
/// </summary>
/// <typeparam name="TNative">The native platform view type</typeparam>
/// <typeparam name="TVirtual">The virtual view type implementing IView</typeparam>
public abstract class PlatformHandlerBase<TNative, TVirtual> : IPlatformHandler<TNative, TVirtual>
    where TNative : class
    where TVirtual : IView
{
    private bool _isConnected;
    private TNative? _platformView;
    private TVirtual? _virtualView;

    /// <summary>
    /// Gets the native platform view instance
    /// </summary>
    public TNative? PlatformView => _platformView;

    /// <summary>
    /// Gets the virtual view instance
    /// </summary>
    public TVirtual? VirtualView => _virtualView;

    /// <summary>
    /// Gets whether the handler is currently connected
    /// </summary>
    protected bool IsConnected => _isConnected;

    /// <summary>
    /// Creates the native platform view for the handler
    /// </summary>
    /// <returns>The created native view instance</returns>
    public abstract TNative CreatePlatformView();

    /// <summary>
    /// Connects the handler by linking the virtual view to the native view
    /// </summary>
    /// <param name="virtualView">The virtual view to connect</param>
    public virtual void ConnectHandler(TVirtual virtualView)
    {
        if (_isConnected)
        {
            DisconnectHandler();
        }

        _virtualView = virtualView ?? throw new ArgumentNullException(nameof(virtualView));
        _platformView = CreatePlatformView();
        
        OnConnected();
        _isConnected = true;
    }

    /// <summary>
    /// Disconnects the handler and cleans up resources
    /// </summary>
    public virtual void DisconnectHandler()
    {
        if (!_isConnected)
        {
            return;
        }

        OnDisconnecting();
        
        _virtualView = default;
        
        if (_platformView is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        _platformView = null;
        _isConnected = false;
        
        OnDisconnected();
    }

    /// <summary>
    /// Called when the handler has been connected
    /// </summary>
    protected virtual void OnConnected()
    {
    }

    /// <summary>
    /// Called before the handler is disconnected
    /// </summary>
    protected virtual void OnDisconnecting()
    {
    }

    /// <summary>
    /// Called after the handler has been disconnected
    /// </summary>
    protected virtual void OnDisconnected()
    {
    }

    /// <summary>
    /// Updates a property on the native view
    /// </summary>
    /// <param name="propertyName">The name of the property being updated</param>
    protected virtual void UpdateProperty(string propertyName)
    {
        if (!_isConnected || _platformView == null || _virtualView == null)
        {
            return;
        }

        OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// Called when a property changes that requires native view updates
    /// </summary>
    /// <param name="propertyName">The name of the property that changed</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
    }
}
