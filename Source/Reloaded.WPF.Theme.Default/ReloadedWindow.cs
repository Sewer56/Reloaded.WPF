using System.Windows;

namespace Reloaded.WPF.Theme.Default
{
    /// <inheritdoc />
    public class ReloadedWindow : Window
    {
        /// <summary>
        /// Returns the ViewModel ( <see cref="WindowViewModel"/> ) of this window that allows you to toggle window properties.
        /// </summary>
        public WindowViewModel ViewModel => (WindowViewModel) this.DataContext;

        /// <inheritdoc />
        public ReloadedWindow()
        {
            this.DataContext = new WindowViewModel(this);
        }

    }
}
