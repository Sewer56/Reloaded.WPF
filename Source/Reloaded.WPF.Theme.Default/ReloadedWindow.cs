using System.Windows;

namespace Reloaded.WPF.Theme.Default
{
    public class ReloadedWindow : Window
    {
        /// <summary>
        /// Returns the ViewModel ( <see cref="WindowViewModel"/> ) of this window that allows you to toggle window properties.
        /// </summary>
        public WindowViewModel ViewModel => (WindowViewModel) this.DataContext;

        public ReloadedWindow()
        {
            this.DataContext = new WindowViewModel(this);
        }

    }
}
