using Microsoft.Maui;

namespace Brupper.Maui.Handlers;

/// <summary>
/// Base interface for platform-specific handlers that bridge virtual views to native platform views
/// </summary>
/// <typeparam name="TNative">The native platform view type</typeparam>
/// <typeparam name="TVirtual">The virtual view type</typeparam>
public interface IPlatformHandler<TNative, TVirtual>
    where TNative : class
    where TVirtual : IView
{
    /// <summary>
    /// Gets the native platform view instance
    /// </summary>
    TNative? PlatformView { get; }

    /// <summary>
    /// Gets the virtual view instance
    /// </summary>
    TVirtual? VirtualView { get; }

    /// <summary>
    /// Creates the native platform view for the handler
    /// </summary>
    /// <returns>The created native view instance</returns>
    TNative CreatePlatformView();

    /// <summary>
    /// Connects the handler by linking the virtual view to the native view
    /// </summary>
    /// <param name="virtualView">The virtual view to connect</param>
    void ConnectHandler(TVirtual virtualView);

    /// <summary>
    /// Disconnects the handler and cleans up resources
    /// </summary>
    void DisconnectHandler();
}
