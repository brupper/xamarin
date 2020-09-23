using MvvmCross;
using MvvmCross.Localization;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Brupper.Forms.Converters
{
    public class TranslateConverter : IValueConverter
    {
        #region Fields

        private IMvxTextProvider textProvider;

        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string resourceId = value as string;
            if (value == null || string.IsNullOrEmpty(resourceId) && !(value is Enum))
            {
                return string.Empty;
            }
            else if (value is Enum)
            {
                resourceId = $"{value.GetType().Name}.{value}";
            }

            if (!string.IsNullOrEmpty(resourceId))
            {
                if (parameter is string prefix)
                {
                    resourceId = prefix + resourceId;
                }

                if (textProvider == null)
                {
                    textProvider = Mvx.IoCProvider.GetSingleton<IMvxTextProvider>();
                }

                var localizedText = textProvider.GetText(null, null, resourceId);
                if (localizedText == null)
                {
#if DEBUG
                    throw new ArgumentNullException($"'{resourceId}' was not found in Recources for culture '{CultureInfo.CurrentUICulture.Name ?? "-"}'");
#else
                    localizedText = "<??? - resourceId>"; // resource's not found
#endif
                }

                return localizedText;
            }

            return "<???>"; // resource's not found
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
