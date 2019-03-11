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
using Reloaded.WPF.Annotations;

namespace Reloaded.WPF.Controls
{
    /// <summary>
    /// Interaction logic for CircleButton.xaml
    /// </summary>
    public partial class CircleButton : UserControl
    {
        /// <summary>
        /// The source of the image 
        /// </summary>
        public String ImageSource { get; set; }

        /// <summary>
        /// Gets or Sets the text displayed by the tooltip.
        /// </summary>
        public string TooltipText { get; set; }

        /// <summary>
        /// Retrieves the internal button control.
        /// </summary>
        public Button Button => this._button;

        public CircleButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
