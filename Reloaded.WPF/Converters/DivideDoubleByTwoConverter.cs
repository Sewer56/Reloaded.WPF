using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Reloaded.WPF.Converters
{
    /// <summary>
    /// Accepts an integer, returns integer divided by two.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class DivideDoubleByTwoConverter : IValueConverter
    {
        public static DivideDoubleByTwoConverter Instance = new DivideDoubleByTwoConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                double val = (double)value;
                return val / 2.0F;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
