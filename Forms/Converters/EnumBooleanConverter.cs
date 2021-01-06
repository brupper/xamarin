using System;
using Xamarin.Forms;

namespace Brupper.Forms.Converters
{
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string s && parameter is Enum enumValue)
            {
                return s.Contains($"{enumValue.GetType().Name}.{parameter}");
            }
            else if (value is Enum valueEnum && parameter is Enum parameterEnum)
            {
                return (valueEnum.GetType().IsDefined(typeof(FlagsAttribute), inherit: false) && valueEnum.HasFlag(parameterEnum)) || valueEnum.Equals(parameterEnum);
            }
            return value?.Equals(parameter) ?? false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? parameter : Binding.DoNothing;
            }

            return Binding.DoNothing;
        }
    }
}
