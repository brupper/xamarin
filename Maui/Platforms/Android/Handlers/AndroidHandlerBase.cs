using Android.Content;
using AndroidView = Android.Views.View;
using Microsoft.Maui;
using Microsoft.Maui.Platform;

namespace Brupper.Maui.Handlers;

/// <summary>
/// Base class for Android platform handlers
/// </summary>
/// <typeparam name="TNative">The native Android View type</typeparam>
/// <typeparam name="TVirtual">The virtual view type</typeparam>
public abstract class AndroidHandlerBase<TNative, TVirtual> : PlatformHandlerBase<TNative, TVirtual>
    where TNative : AndroidView
    where TVirtual : IView
{
    /// <summary>
    /// Gets the Android context for creating views
    /// </summary>
    protected Context Context => Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.BaseContext 
        ?? throw new InvalidOperationException("Android context is not available");

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
    /// Converts a MAUI color to an Android color
    /// </summary>
    /// <param name="color">The MAUI color</param>
    /// <returns>The Android color integer</returns>
    protected static int ToAndroidColor(Microsoft.Maui.Graphics.Color? color)
    {
        return color?.ToPlatform() ?? Android.Graphics.Color.Transparent;
    }

    /// <summary>
    /// Converts a density-independent pixel value to actual pixels
    /// </summary>
    /// <param name="dp">The dp value</param>
    /// <returns>The pixel value</returns>
    protected int DpToPixels(double dp)
    {
        return (int)(dp * Context.Resources?.DisplayMetrics?.Density ?? 1.0);
    }

    /// <summary>
    /// Converts pixels to density-independent pixels
    /// </summary>
    /// <param name="pixels">The pixel value</param>
    /// <returns>The dp value</returns>
    protected double PixelsToDp(int pixels)
    {
        return pixels / (Context.Resources?.DisplayMetrics?.Density ?? 1.0);
    }
}
