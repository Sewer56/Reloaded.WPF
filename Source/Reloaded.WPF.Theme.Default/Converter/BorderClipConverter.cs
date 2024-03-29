﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Reloaded.WPF.Theme.Default.Converter
{
    /// <summary>
    /// Creates a clip region for a border provided the border's width, height and corner radius are bound.
    /// </summary>
    public class BorderClipConverter : IMultiValueConverter
    {
        // Inspired by: https://stackoverflow.com/a/5650367/11106111
        public static BorderClipConverter Instance = new BorderClipConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double && values[1] is double && values[2] is CornerRadius && values.Length == 3)
            {
                var width  = (double) values[0];
                var height = (double) values[1];

                if (width < double.Epsilon || height < double.Epsilon)
                    return Geometry.Empty;

                var radius = (CornerRadius)values[2];

                // Actually we need more complex geometry, when CornerRadius has different values.
                // But let me not to take this into account, and simplify example for a common value.
                var clip = new RectangleGeometry(new Rect(0, 0, width, height), radius.TopLeft, radius.TopLeft);
                clip.Freeze();
                return clip;
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
