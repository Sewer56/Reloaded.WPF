using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Reloaded.WPF.Resources;

namespace Reloaded.WPF.Controls
{
    /// <summary>
    /// The <see cref="CircleButton"/> is a specialized type of control that hosts an image, clipped
    /// such that its shape is a circle that supports a tooltip.
    /// </summary>
    public partial class CircleButton : UserControl
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(CircleButton), new PropertyMetadata(new BitmapImage(new Uri(Paths.PLACEHOLDER_IMAGE, UriKind.RelativeOrAbsolute))));
        public static readonly DependencyProperty TooltipTextProperty = DependencyProperty.Register(nameof(TooltipText), typeof(String), typeof(CircleButton), new PropertyMetadata("Tooltip Text (Binding)"));
        public static readonly DependencyProperty BitmapScalingProperty = DependencyProperty.Register(nameof(BitmapScaleMode), typeof(BitmapScalingMode), typeof(CircleButton), new PropertyMetadata(BitmapScalingMode.HighQuality));

        /// <summary>
        /// The source of the image.
        /// </summary>
        public ImageSource ImageSource
        {
            get => (ImageSource) GetValue(ImageSourceProperty);
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
        /// Gets or sets the bitmap scaling mode for the image.
        /// </summary>
        public BitmapScalingMode BitmapScaleMode
        { 
            get => (BitmapScalingMode)GetValue(BitmapScalingProperty);
            set => SetValue(BitmapScalingProperty, value);
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
