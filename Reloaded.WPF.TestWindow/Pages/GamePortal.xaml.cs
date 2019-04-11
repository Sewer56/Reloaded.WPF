using System.Windows;
using Reloaded.WPF.TestWindow.Models.ViewModel;
using Reloaded.WPF.Theme.Default;

namespace Reloaded.WPF.TestWindow.Pages
{
    /// <summary>
    /// The main page of the application.
    /// </summary>
    public partial class GamePortal : ReloadedPage
    {
        public GamePortal()
        {
            InitializeComponent();
            this.DataContext = IoC.Get<CurrentProcess>();
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
