#pragma warning disable 1591

using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Reloaded.WPF.Theme.Default.Converter
{
    /// <summary>
    /// Converts a Colour of a <see cref="Color"/> from sRGB space into a colour of the RGB space.
    /// </summary>
    /// <remarks>
    /// There seems to be an issue in the <see cref="DropShadowEffect"/> implementation
    /// whereby it converts all colours assigned to it into the sRGB spectrum for
    /// an unknown reason. This converter undoes an RGB to sRGB colour conversion
    /// such that once the <see cref="DropShadowEffect"/> does the sRGB conversion, the
    /// resulting colour will be RGB as intended.
    /// </remarks>
    public class ColorToShadowColorConverter : IValueConverter
    {
        /* Answer inspired by the following StackOverflow question: https://stackoverflow.com/questions/22835131/wpf-how-to-set-desired-color-for-shadow */

        public static ColorToShadowColorConverter Instance = new ColorToShadowColorConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color color)
            {
                var r = Transform(color.R);
                var g = Transform(color.G);
                var b = Transform(color.B);

                return Color.FromArgb(color.A, r, g, b);
            }

            return value;
        }


        /// <summary>
        /// Converts a colour from the RGB to the sRGB colour space.
        /// See: http://en.wikipedia.org/wiki/SRGB
        /// </summary>
        /// <param name="source">The colour to convert from sRGB to RGB</param>
        private byte Transform(byte source)
        {
            double normalized = source / 255.0;
            double gammaExponent = 1.0 / 2.2;
            double finalColor = Math.Pow(normalized, gammaExponent);

            return (byte)Math.Round(finalColor * 255.0, MidpointRounding.AwayFromZero);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(ColorToShadowColorConverter)} is a OneWay converter.");
        }
    }
}
