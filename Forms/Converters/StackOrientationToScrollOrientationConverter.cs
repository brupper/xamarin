using System;
using System.Globalization;
using Xamarin.Forms;

namespace Brupper.Froms.Converters
{
    public class StackOrientationToScrollOrientationConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StackOrientation orientation)
            {
                if (orientation == StackOrientation.Horizontal)
                {
                    return ScrollOrientation.Horizontal;
                }
                else if (orientation == StackOrientation.Vertical)
                {
                    return ScrollOrientation.Vertical;
                }
            }

            return ScrollOrientation.Both;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ScrollOrientation orientation)
            {
                if (orientation == ScrollOrientation.Horizontal)
                {
                    return StackOrientation.Horizontal;
                }
                else if (orientation == ScrollOrientation.Vertical)
                {
                    return StackOrientation.Vertical;
                }
            }

            return StackOrientation.Vertical;
        }
    }
}
