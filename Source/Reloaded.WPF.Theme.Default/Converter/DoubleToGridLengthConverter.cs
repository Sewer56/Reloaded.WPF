﻿#pragma warning disable 1591

using System;
using System.Globalization;
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
            try
            {
                if (value is double)
                    return new System.Windows.GridLength((double)value);
            }
            catch (Exception ex)
            {
                return null;
            }
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
