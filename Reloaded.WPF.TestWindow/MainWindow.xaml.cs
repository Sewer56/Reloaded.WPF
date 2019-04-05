using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PropertyChanged;
using Reloaded.WPF.Controls;
using Reloaded.WPF.Theme.Default.ViewModels;
using Reloaded.WPF.Utilities;

namespace Reloaded.WPF.TestWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The currently displayed page on this window.
        /// </summary>
        public Pages.Page CurrentPage { get; set; } = Pages.Page.Base;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new WindowViewModel(this);
        }
    }
}
