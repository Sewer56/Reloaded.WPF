using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Reloaded.WPF.Theme.Default.Converter
{
    /// <summary>
    /// Converts a <see cref="System.Double"/> to a <see cref="GridLength"/>.
    /// </summary>
    public class DoubleToGridLengthConverter : IValueConverter
    {
        public static DoubleToGridLengthConverter Instance = new DoubleToGridLengthConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
                return new System.Windows.GridLength((double)value);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
