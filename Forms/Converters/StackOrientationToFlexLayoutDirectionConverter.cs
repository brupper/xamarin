using System;
using System.Globalization;
using Xamarin.Forms;

namespace Brupper.Froms.Converters
{
    public class StackOrientationToFlexLayoutDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StackOrientation orientation)
            {
                if (orientation == StackOrientation.Horizontal)
                {
                    return FlexDirection.Row;
                }
                else if (orientation == StackOrientation.Vertical)
                {
                    return FlexDirection.Column;
                }
            }

            return FlexDirection.Column;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FlexDirection orientation)
            {
                if (orientation == FlexDirection.Row)
                {
                    return StackOrientation.Horizontal;
                }
                else if (orientation == FlexDirection.Column)
                {
                    return StackOrientation.Vertical;
                }
            }

            return StackOrientation.Vertical;
        }
    }
}
