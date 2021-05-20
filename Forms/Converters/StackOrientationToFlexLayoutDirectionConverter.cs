using System;
using System.Globalization;
using Xamarin.Forms;

namespace Brupper.Forms.Converters
{
    public class StackOrientationToFlexLayoutDirectionConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
