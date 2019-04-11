using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Reloaded.WPF.Resources;

namespace Reloaded.WPF.Controls.DesignTimeModels
{
    public class CircleButtonModel
    {
        public static CircleButtonModel Instance { get; set; } = new CircleButtonModel();

        public ImageSource ImageSource { get; set; } = new BitmapImage(new Uri(Paths.PLACEHOLDER_IMAGE));
        public String TooltipText { get; set; } = "Sample Tooltip Text";
    }
}
