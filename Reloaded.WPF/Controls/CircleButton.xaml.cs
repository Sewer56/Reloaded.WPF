using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PropertyChanged;

namespace Reloaded.WPF.Controls
{
    /// <summary>
    /// Interaction logic for CircleButton.xaml
    /// </summary>
    public partial class CircleButton : UserControl
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(CircleButton), new PropertyMetadata(new BitmapImage(new Uri("/Reloaded.WPF;component/Images/Reloaded_Icon.png", UriKind.RelativeOrAbsolute))));
        public static readonly DependencyProperty TooltipTextProperty = DependencyProperty.Register(nameof(TooltipText), typeof(String), typeof(CircleButton), new PropertyMetadata("Tooltip Text"));

        /// <summary>
        /// The source of the image.
        /// </summary>
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        /// Gets or Sets the text displayed by the tooltip.
        /// </summary>
        public string TooltipText
        {
            get => (string)GetValue(TooltipTextProperty);
            set => SetValue(TooltipTextProperty, value);
        }

        /// <summary>
        /// Retrieves the internal button control.
        /// </summary>
        public Button Button => this._button;

        public CircleButton()
        {
            InitializeComponent();
        }
    }
}
