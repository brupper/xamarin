using Brupper.Forms.Effects;
using Brupper.Forms.Platforms.iOS.Effects;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(PickerPlatformEffect), nameof(PickerEffect))]
namespace Brupper.Forms.Platforms.iOS.Effects
{
    public class PickerPlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            UpdateUIProperties();
        }

        protected override void OnDetached()
        {
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == Forms.Effects.PickerEffect.BackgroundColorProperty.PropertyName ||
                args.PropertyName == Forms.Effects.PickerEffect.BorderColorProperty.PropertyName ||
                args.PropertyName == Forms.Effects.PickerEffect.BorderWidthProperty.PropertyName ||
                args.PropertyName == Forms.Effects.PickerEffect.CornerRadiusProperty.PropertyName)
            {
                UpdateUIProperties();
            }
        }

        private void UpdateUIProperties()
        {
            if (Control is UITextField textField)
            {
                textField.BackgroundColor = Forms.Effects.PickerEffect.GetBackgroundColor(Element).ToUIColor();
                textField.Layer.BorderColor = Forms.Effects.PickerEffect.GetBorderColor(Element).ToCGColor();
                textField.Layer.BorderWidth = Forms.Effects.PickerEffect.GetBorderWidth(Element);
                textField.Layer.CornerRadius = Forms.Effects.PickerEffect.GetCornerRadius(Element);
            }
        }
    }
}
