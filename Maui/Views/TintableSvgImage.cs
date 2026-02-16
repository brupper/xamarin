//using FFImageLoading.Svg.Forms;
//using FFImageLoading.Transformations;
//using FFImageLoading.Work;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SkiaSharp.Extended.Svg;
using SkiaSharp.Views.Maui.Controls;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;
using Color = Microsoft.Maui.Graphics.Color;

namespace Brupper.Forms.Views;

public class TintableSvgImage : SKCanvasView //SvgCachedImage
{
    private volatile bool animationInProgress;

    public ImageSource Source
    {
        set
        {
            // TODO: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/behaviors/icon-tint-color-behavior

            var source = value;
            // using var stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
            // return stream;
        }
    }

    #region TintColorProperty

    public static readonly BindableProperty TintColorProperty
        = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(TintableSvgImage), Colors.Transparent, propertyChanged: UpdateColor);

    public Color TintColor
    {
        get { return (Color)GetValue(TintColorProperty); }
        set { SetValue(TintColorProperty, value); }
    }

    private static void UpdateColor(BindableObject bindable, object oldColor, object newColor)
    {
        var oldcolor = (Color)oldColor;
        var newcolor = (Color)newColor;

        if (!oldcolor.Equals(newcolor))
        {
            var view = (TintableSvgImage)bindable;

            // view.TintableSvgImage_PaintSurface(view, new SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs());
        }
    }

    #endregion

    #region CommandProperty

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(TintableSvgImage), null);

    #endregion

    public TintableSvgImage()
    {
        var tapped = new TapGestureRecognizer();
        tapped.Tapped += Handle_Tapped;
        GestureRecognizers.Add(tapped);

        this.PaintSurface += TintableSvgImage_PaintSurface;
    }

    private void TintableSvgImage_PaintSurface(object? sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
        var surface = e.Surface;
        var canvas = surface.Canvas;

        // https://stackoverflow.com/questions/74334591/specify-the-color-of-a-svg-image-in-net-maui
        canvas.Clear();
        //var stream = LoadStream(typeof(MainPage), "myfile.svg");
        
        var svg = new SKSvg();
        // svg.Load(stream);

        // https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/behaviors/icon-tint-color-behavior
        using (var paint = new SkiaSharp.SKPaint())
        {
            TintColor.ToRgba(out var r, out var g, out var b, out var a);
            paint.ColorFilter = SkiaSharp.SKColorFilter.CreateBlendMode(new SkiaSharp.SKColor(r, g, b, a), SkiaSharp.SKBlendMode.SrcIn);
            canvas.DrawPicture(svg.Picture, paint);
        }
    }

    private async void Handle_Tapped(object sender, EventArgs e)
    {
        if (animationInProgress)
        {
            return;
        }

        _ = Task.Delay(150).ContinueWith(t =>
        {
            Command?.Execute(BindingContext);
        }).ConfigureAwait(false);

        animationInProgress = true;
        await this.FadeTo(0.4, 50);
        await this.FadeTo(1, 50);
        animationInProgress = false;
    }
}
