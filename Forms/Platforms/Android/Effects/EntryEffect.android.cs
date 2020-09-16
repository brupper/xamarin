using Android.Graphics.Drawables;
using Brupper.Forms.Effects;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("Brupper")]
[assembly: ExportEffect(typeof(Brupper.Forms.Platforms.Android.Effects.EntryEffect), nameof(EntryEffect))]
namespace Brupper.Forms.Platforms.Android.Effects
{
    public class EntryEffect : PlatformEffect
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
            drawable.SetColor(Forms.Effects.EntryEffect.GetBackgroundColor(Element).ToAndroid());
            drawable.SetStroke(Forms.Effects.EntryEffect.GetBorderWidth(Element), Forms.Effects.EntryEffect.GetBorderColor(Element).ToAndroid());
            drawable.SetCornerRadius(Forms.Effects.EntryEffect.GetCornerRadius(Element));

            Control.SetBackground(drawable);
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == Forms.Effects.EntryEffect.BackgroundColorProperty.PropertyName ||
                args.PropertyName == Forms.Effects.EntryEffect.BorderColorProperty.PropertyName ||
                args.PropertyName == Forms.Effects.EntryEffect.BorderWidthProperty.PropertyName ||
                args.PropertyName == Forms.Effects.EntryEffect.CornerRadiusProperty.PropertyName)
            {
                UpdateDrawable();
            }
        }
    }

}