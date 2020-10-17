using Android.Graphics.Drawables;
using Android.Runtime;
using Brupper.Forms.Effects;
using Brupper.Forms.Platforms.Android.Effects;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(PickerPlatformEffect), nameof(PickerEffect))]
namespace Brupper.Forms.Platforms.Android.Effects
{
    [Preserve(AllMembers = true)]
    public class PickerPlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            UpdateDrawable();
        }

        protected override void OnDetached()
        {
        }

        private void UpdateDrawable()
        {
            var drawable = new GradientDrawable();
            drawable.SetColor(Forms.Effects.PickerEffect.GetBackgroundColor(Element).ToAndroid());
            drawable.SetStroke(Forms.Effects.PickerEffect.GetBorderWidth(Element), Forms.Effects.PickerEffect.GetBorderColor(Element).ToAndroid());
            drawable.SetCornerRadius(Forms.Effects.PickerEffect.GetCornerRadius(Element));

            Control.SetBackground(drawable);
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == Forms.Effects.PickerEffect.BackgroundColorProperty.PropertyName ||
                args.PropertyName == Forms.Effects.PickerEffect.BorderColorProperty.PropertyName ||
                args.PropertyName == Forms.Effects.PickerEffect.BorderWidthProperty.PropertyName ||
                args.PropertyName == Forms.Effects.PickerEffect.CornerRadiusProperty.PropertyName)
            {
                UpdateDrawable();
            }
        }
    }
}
