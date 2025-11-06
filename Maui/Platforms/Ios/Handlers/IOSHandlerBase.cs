using Microsoft.Maui;
using Microsoft.Maui.Platform;
using UIKit;

namespace Brupper.Maui.Handlers;

/// <summary>
/// Base class for iOS platform handlers
/// </summary>
/// <typeparam name="TNative">The native UIView type</typeparam>
/// <typeparam name="TVirtual">The virtual view type</typeparam>
public abstract class IOSHandlerBase<TNative, TVirtual> : PlatformHandlerBase<TNative, TVirtual>
    where TNative : UIView
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
    /// Converts a MAUI color to a UIColor
    /// </summary>
    /// <param name="color">The MAUI color</param>
    /// <returns>The UIColor instance</returns>
    protected static UIColor ToUIColor(Microsoft.Maui.Graphics.Color? color)
    {
        return color?.ToPlatform() ?? UIColor.Clear;
    }

    /// <summary>
    /// Converts a MAUI font to a UIFont
    /// </summary>
    /// <param name="font">The MAUI font</param>
    /// <param name="defaultSize">The default font size if not specified</param>
    /// <returns>The UIFont instance</returns>
    protected static UIFont ToUIFont(Microsoft.Maui.Font font, double defaultSize = 17)
    {
        var size = (nfloat)(font.Size > 0 ? font.Size : defaultSize);
        
        if (!string.IsNullOrEmpty(font.Family))
        {
            var uiFont = UIFont.FromName(font.Family, size);
            if (uiFont != null)
            {
                return uiFont;
            }
        }

        return (int)font.Weight switch
        {
            >= 700 => UIFont.BoldSystemFontOfSize(size),
            _ => UIFont.SystemFontOfSize(size)
        };
    }

    /// <summary>
    /// Converts points to pixels for the current screen scale
    /// </summary>
    /// <param name="points">The point value</param>
    /// <returns>The pixel value</returns>
    protected nfloat PointsToPixels(double points)
    {
        return (nfloat)points * UIScreen.MainScreen.Scale;
    }

    /// <summary>
    /// Converts pixels to points for the current screen scale
    /// </summary>
    /// <param name="pixels">The pixel value</param>
    /// <returns>The point value</returns>
    protected double PixelsToPoints(nfloat pixels)
    {
        return (double)(pixels / UIScreen.MainScreen.Scale);
    }
}
