using System.Linq;
using Xamarin.Forms;

namespace Brupper.Forms.Effects
{
    public class PickerEffect : RoutingEffect
    {
        #region Constructor

        public PickerEffect()
            : base($"Brupper.{nameof(PickerEffect)}")
        {
        }

        #endregion

        #region BackgroundColorProperty

        public static readonly BindableProperty BackgroundColorProperty
                = BindableProperty.CreateAttached("BackgroundColor", typeof(Color), typeof(PickerEffect), Color.White, propertyChanged: OnPropertyChanged);

        public static Color GetBackgroundColor(BindableObject bindable)
        {
            return (Color)bindable.GetValue(BackgroundColorProperty);
        }

        public static void SetBackgroundColor(BindableObject bindable, Color value)
        {
            bindable.SetValue(BackgroundColorProperty, value);
        }

        #endregion

        #region BorderColorProperty

        public static readonly BindableProperty BorderColorProperty
                = BindableProperty.CreateAttached("BorderColor", typeof(Color), typeof(PickerEffect), Color.White, propertyChanged: OnPropertyChanged);

        public static Color GetBorderColor(BindableObject bindable)
        {
            return (Color)bindable.GetValue(BorderColorProperty);
        }

        public static void SetBorderColor(BindableObject bindable, Color value)
        {
            bindable.SetValue(BorderColorProperty, value);
        }

        #endregion

        #region CornerRadiusProperty

        public static readonly BindableProperty CornerRadiusProperty
                = BindableProperty.CreateAttached("CornerRadius", typeof(int), typeof(PickerEffect), 0, propertyChanged: OnPropertyChanged);

        public static int GetCornerRadius(BindableObject bindable)
        {
            return (int)bindable.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(BindableObject bindable, int value)
        {
            bindable.SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region BorderWidthProperty

        public static readonly BindableProperty BorderWidthProperty
                = BindableProperty.CreateAttached("BorderWidth", typeof(int), typeof(PickerEffect), 0, propertyChanged: OnPropertyChanged);

        public static int GetBorderWidth(BindableObject bindable)
        {
            return (int)bindable.GetValue(BorderWidthProperty);
        }

        public static void SetBorderWidth(BindableObject bindable, int value)
        {
            bindable.SetValue(BorderWidthProperty, value);
        }

        #endregion

        #region Private Methods

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is VisualElement view)
            {
                var effect = view.Effects.FirstOrDefault(x => x is PickerEffect);
                if (effect == null)
                {
                    view.Effects.Add(new PickerEffect());
                }
            }
        }

        #endregion
    }
}
