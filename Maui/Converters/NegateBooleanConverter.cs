using Microsoft.Maui.Controls;

namespace Brupper.Maui.Converters;

public class NegateBooleanConverter : IValueConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool b)
        {
            return !b;
        }

        return false;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return !(bool)value;
    }
}
