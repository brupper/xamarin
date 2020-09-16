using Brupper.Forms.Effects;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Brupper")]
[assembly: ExportEffect(typeof(Brupper.Forms.Platforms.iOS.Effects.EntryEffect), nameof(EntryEffect))]
namespace Brupper.Forms.Platforms.iOS.Effects
{
    public class EntryEffect : PlatformEffect
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

            if (args.PropertyName == Forms.Effects.EntryEffect.BackgroundColorProperty.PropertyName ||
                args.PropertyName == Forms.Effects.EntryEffect.BorderColorProperty.PropertyName ||
                args.PropertyName == Forms.Effects.EntryEffect.BorderWidthProperty.PropertyName ||
                args.PropertyName == Forms.Effects.EntryEffect.CornerRadiusProperty.PropertyName)
            {
                UpdateUIProperties();
            }
        }

        private void UpdateUIProperties()
        {
            if (Control is UITextField textField)
            {
                textField.BackgroundColor = Forms.Effects.EntryEffect.GetBackgroundColor(Element).ToUIColor();
                textField.Layer.BorderColor = Forms.Effects.EntryEffect.GetBorderColor(Element).ToCGColor();
                textField.Layer.BorderWidth = Forms.Effects.EntryEffect.GetBorderWidth(Element);
                textField.Layer.CornerRadius = Forms.Effects.EntryEffect.GetCornerRadius(Element);
            }
            else if (Control is UITextView textView)
            {
                textView.BackgroundColor = Forms.Effects.EntryEffect.GetBackgroundColor(Element).ToUIColor();
                textView.Layer.BorderColor = Forms.Effects.EntryEffect.GetBorderColor(Element).ToCGColor();
                textView.Layer.BorderWidth = Forms.Effects.EntryEffect.GetBorderWidth(Element);
                textView.Layer.CornerRadius = Forms.Effects.EntryEffect.GetCornerRadius(Element);
            }
            //if (Control is Views.FloatLabeledTextField floatLabeledTextField)
            //{
            //    floatLabeledTextField.UseTextInputLayout = Forms.Effects.EntryEffect.GetUseTextInputLayout(Element);
            //}
        }
    }

}