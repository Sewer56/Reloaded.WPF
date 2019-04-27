#pragma warning disable 1591

using System;
using System.Globalization;
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
            /*
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return null;
            */

            if (value is string uriString)
            {
                return new BitmapImage(new Uri(uriString, UriKind.RelativeOrAbsolute));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
