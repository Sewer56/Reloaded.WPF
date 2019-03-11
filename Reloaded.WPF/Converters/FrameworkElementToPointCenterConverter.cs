using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Reloaded.WPF.Converters
{
    /// <summary>
    /// Converts a WPF Framework element to a point being the center of the element.
    /// </summary>
    [ValueConversion(typeof(FrameworkElement), typeof(Point))]
    public class FrameworkElementToPointCenterConverter : IValueConverter
    {
        public static FrameworkElementToPointCenterConverter Instance = new FrameworkElementToPointCenterConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FrameworkElement)
            {
                var element = (FrameworkElement) value;
                return new Point(element.ActualWidth / 2, element.ActualHeight / 2);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
