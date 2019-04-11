using System.Windows;

namespace Reloaded.WPF.Theme.Default
{
    public class ReloadedWindow : Window
    {
        public ReloadedWindow()
        {
            this.DataContext = new WindowViewModel(this);
        }

    }
}
