using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Reloaded.WPF.Converters
{
    /// <summary>
    /// Converts an absolute WPF path to a BitmapImage.
    /// </summary>
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class AbsolutePathToImageConverter : IValueConverter
    {
        public static AbsolutePathToImageConverter Instance = new AbsolutePathToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string uriString)
            {
                return new BitmapImage(new Uri(uriString, UriKind.Absolute));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
