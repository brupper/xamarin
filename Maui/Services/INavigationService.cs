using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brupper.Maui.Services;

public interface INavigationService
{
    /// <summary>
    /// 	Performs navigation to one of two pages when the app is launched.
    /// </summary>
    /// <returns></returns>
    Task InitializeAsync();

    /// <summary>
    /// Performs hierarchical navigation to a specified page using a registered
    /// navigation route. Can optionally pass named route parameters to use for 
    /// processing on the destination page
    /// </summary>
    /// <param name="route"></param>
    /// <param name="routeParameters"></param>
    /// <returns></returns>
    Task NavigateToAsync(string route, IDictionary<string, object>? routeParameters = null);

    /// <summary>
    /// Removes the current page from the navigation stack.
    /// </summary>
    /// <returns></returns>
    Task PopAsync();
}
