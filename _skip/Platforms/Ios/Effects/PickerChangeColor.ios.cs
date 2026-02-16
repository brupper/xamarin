using Brupper.Forms.Effects;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Graphics.Converters;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls.Platform;
using System.ComponentModel;
using UIKit;

[assembly: ExportEffect(typeof(Brupper.Forms.Platforms.iOS.Effects.PickerPlatformEffect), nameof(PickerEffect))]
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
                textField.BackgroundColor = Forms.Effects.PickerEffect.GetBackgroundColor(Element).ToPlatform();
                textField.Layer.BorderColor = Forms.Effects.PickerEffect.GetBorderColor(Element).ToCGColor();
                textField.Layer.BorderWidth = Forms.Effects.PickerEffect.GetBorderWidth(Element);
                textField.Layer.CornerRadius = Forms.Effects.PickerEffect.GetCornerRadius(Element);
            }
        }
    }
}
