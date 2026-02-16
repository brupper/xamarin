using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Brupper.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SvgImageButton
{
    private volatile bool animationInProgress;

    #region CommandProperty

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SvgImageButton));

    #endregion

    #region SourceProperty

    public ImageSource Source
    {
        get => (ImageSource)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly BindableProperty SourceProperty =
        BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(SvgImageButton));

    #endregion

    #region TintColorProperty

    public static readonly BindableProperty TintColorProperty 
        = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(SvgImageButton), Colors.Transparent, propertyChanged: UpdateColor);

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
            //var view = (SvgImageButton)bindable;
            //var transformations = new System.Collections.Generic.List<ITransformation>
            //{
            //    new TintTransformation((int)(newcolor.R * 255), (int)(newcolor.G * 255), (int)(newcolor.B * 255), (int)(newcolor.A * 255))
            //    {
            //        EnableSolidColor = true
            //    }
            //};
            //view.Icon.Transformations = transformations;
        }
    }

    #endregion

    #region IsAnimationEnabledProperty

    public bool IsAnimationEnabled
    {
        get => (bool)GetValue(IsAnimationEnabledProperty);
        set => SetValue(IsAnimationEnabledProperty, value);
    }

    public static readonly BindableProperty IsAnimationEnabledProperty =
        BindableProperty.Create(nameof(IsAnimationEnabled), typeof(bool), typeof(SvgImageButton), true);

    #endregion

    public SvgImageButton()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == HeightRequestProperty.PropertyName)
        {
            MiddleFrame.CornerRadius = (float)((HeightRequest) / 2f);
            Icon.HeightRequest = Icon.WidthRequest = MiddleFrame.CornerRadius;
        }
        else if (propertyName == WidthRequestProperty.PropertyName)
        {
            MiddleFrame.CornerRadius = (float)((WidthRequest) / 2f);
            Icon.HeightRequest = Icon.WidthRequest = MiddleFrame.CornerRadius;
        }
    }

    private async void Handle_Tapped(object sender, EventArgs e)
    {
        if (animationInProgress || !IsAnimationEnabled)
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