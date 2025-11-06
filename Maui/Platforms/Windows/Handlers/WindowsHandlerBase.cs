using Microsoft.Maui;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Brupper.Maui.Handlers;

/// <summary>
/// Base class for Windows platform handlers
/// </summary>
/// <typeparam name="TNative">The native FrameworkElement type</typeparam>
/// <typeparam name="TVirtual">The virtual view type</typeparam>
public abstract class WindowsHandlerBase<TNative, TVirtual> : PlatformHandlerBase<TNative, TVirtual>
    where TNative : FrameworkElement
    where TVirtual : IView
{
    /// <summary>
    /// Gets the MAUI context for platform operations
    /// </summary>
    protected IMauiContext? MauiContext { get; private set; }

    /// <summary>
    /// Connects the handler with the MAUI context
    /// </summary>
    /// <param name="virtualView">The virtual view to connect</param>
    public override void ConnectHandler(TVirtual virtualView)
    {
        MauiContext = virtualView.Handler?.MauiContext;
        base.ConnectHandler(virtualView);
    }

    /// <summary>
    /// Converts a MAUI color to a Windows Brush
    /// </summary>
    /// <param name="color">The MAUI color</param>
    /// <returns>The SolidColorBrush instance</returns>
    protected static SolidColorBrush ToBrush(Graphics.Color? color)
    {
        if (color == null)
        {
            return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
        }

        return color.ToPlatform();
    }

    /// <summary>
    /// Converts a MAUI color to a Windows Color
    /// </summary>
    /// <param name="color">The MAUI color</param>
    /// <returns>The Windows.UI.Color</returns>
    protected static Windows.UI.Color ToWindowsColor(Graphics.Color? color)
    {
        return color?.ToWindowsColor() ?? Microsoft.UI.Colors.Transparent;
    }

    /// <summary>
    /// Converts device-independent pixels to actual pixels
    /// </summary>
    /// <param name="dip">The DIP value</param>
    /// <returns>The pixel value</returns>
    protected double DipToPixels(double dip)
    {
        var scale = Microsoft.UI.Xaml.Media.CompositionTarget.GetCompositionScale(PlatformView);
        return dip * scale;
    }

    /// <summary>
    /// Converts pixels to device-independent pixels
    /// </summary>
    /// <param name="pixels">The pixel value</param>
    /// <returns>The DIP value</returns>
    protected double PixelsToDip(double pixels)
    {
        var scale = Microsoft.UI.Xaml.Media.CompositionTarget.GetCompositionScale(PlatformView);
        return pixels / scale;
    }
}
