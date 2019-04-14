using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Reloaded.WPF.Theme.Default.Converter
{
    /// <summary>
    /// Converts a <see cref="Color"/> to a <see cref="SolidColorBrush"/>.
    /// </summary>
    public class ColorToSolidBrushConverter : IValueConverter
    {
        public static ColorToSolidBrushConverter Instance = new ColorToSolidBrushConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(ColorToSolidBrushConverter)} is a OneWay converter.");
        }
    }
}
