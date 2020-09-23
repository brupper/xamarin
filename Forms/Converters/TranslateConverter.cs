using MvvmCross;
using MvvmCross.Localization;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Brupper.Forms.Converters
{
    //[ValueConversion(typeof(string), typeof(string))]
    public class TranslateConverter : IValueConverter
    {
        public static TranslateConverter Instance { get; } = new TranslateConverter();

        private readonly IMvxTextProvider textProvider;

        public TranslateConverter()
        {
            textProvider = Mvx.IoCProvider.GetSingleton<IMvxTextProvider>();
        }

        public static void Init()
        {
            var time = DateTime.UtcNow;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var text = (string)value;

            var translation = textProvider.GetText(null, null, text);
            //var translation = Labels.ResourceManager.GetString(text);
            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    $"Key '{text}' was not found in resources for culture '{(CultureInfo.CurrentUICulture.Name ?? "<???>")}'.");
#else
				translation = text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
            }
            return translation;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
