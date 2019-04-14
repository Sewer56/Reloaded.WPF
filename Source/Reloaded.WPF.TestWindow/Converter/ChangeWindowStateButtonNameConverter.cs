using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Reloaded.WPF.Theme.Default;

namespace Reloaded.WPF.TestWindow.Converter
{
    /// <summary>
    /// Given an enumerable of type <see cref="WindowViewModel.State"/>, return the button text to be shown
    /// for toggling the window state.
    /// </summary>
    public class ChangeWindowStateButtonNameConverter : IValueConverter
    {
        public static readonly ChangeWindowStateButtonNameConverter Instance = new ChangeWindowStateButtonNameConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (WindowViewModel.State) value;
            return $"State: {value.ToString()}. Click to switch state.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
