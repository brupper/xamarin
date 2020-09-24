using FFImageLoading.Svg.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Brupper.Forms.Views
{
    public class TintableSvgImage : SvgCachedImage
    {
        private volatile bool animationInProgress;

        #region TintColorProperty

        public static BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(TintableSvgImage), Color.Transparent, propertyChanged: UpdateColor);

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
                var transformations = new System.Collections.Generic.List<ITransformation>
                {
                    new TintTransformation((int)(newcolor.R * 255), (int)(newcolor.G * 255), (int)(newcolor.B * 255), (int)(newcolor.A * 255))
                    {
                        EnableSolidColor = true
                    }
                };
                view.Transformations = transformations;
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
}
