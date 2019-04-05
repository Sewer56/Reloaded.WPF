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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Reloaded.WPF.Controls.Page;
using Reloaded.WPF.Theme.Default;

namespace Reloaded.WPF.TestWindow.Pages
{
    /// <summary>
    /// The main page of the application.
    /// </summary>
    public partial class GamePortal : PageBase
    {
        public GamePortal()
        {
            InitializeComponent();
            Loader.Load(this);
        }

        private async void AnimateInClick(object sender, RoutedEventArgs e)
        {
            await this.AnimateIn();
        }

        private async void AnimateOutClick(object sender, RoutedEventArgs e)
        {
            await this.AnimateOut();
            await this.AnimateIn();
        }
    }
}
