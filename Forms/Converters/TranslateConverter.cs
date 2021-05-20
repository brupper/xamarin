using MvvmCross;
using MvvmCross.Localization;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Brupper.Forms.Converters
{
    public class TranslateConverter : IValueConverter
    {
        public const string NotFoundPrefix = "<??? - ";
        public const string NotFoundPostfix = "???>";

        #region Fields

        private IMvxTextProvider textProvider;

        #endregion

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resourceId = value as string;

            if (value == null || string.IsNullOrEmpty(resourceId) && !(value is Enum))
            {
                return $"{NotFoundPrefix}{NotFoundPostfix}";
            }
            else if (value is Enum numericValue)
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

                //var localizedText = Resources.Labels.ResourceManager.GetString(resourceId);
                var localizedText = textProvider.GetText(null, null, resourceId);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    return localizedText;
                }
            }

            return $"{NotFoundPrefix}{resourceId}>"; // resource's not found
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
