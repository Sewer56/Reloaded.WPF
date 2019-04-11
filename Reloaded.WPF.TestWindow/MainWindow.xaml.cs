using Reloaded.WPF.Theme.Default;

namespace Reloaded.WPF.TestWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReloadedWindow
    {
        /// <summary>
        /// The currently displayed page on this window.
        /// </summary>
        public Pages.Page CurrentPage { get; set; } = Pages.Page.Base;

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
