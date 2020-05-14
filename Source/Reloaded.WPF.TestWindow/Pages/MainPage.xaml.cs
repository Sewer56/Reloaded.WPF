using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Reloaded.WPF.TestWindow.Models.ViewModel;
using Reloaded.WPF.Theme.Default;
using Process = Reloaded.WPF.TestWindow.Models.Model.Process;

namespace Reloaded.WPF.TestWindow.Pages
{
    /// <summary>
    /// The main page of the application.
    /// </summary>
    public partial class MainPage : ReloadedPage
    {
        public MainPage() : base()
        {  
            InitializeComponent();
            this.DataContext = IoC.Get<SidebarViewModel>();
        }

        private void CircleButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Prepare for parameter transfer.
            if (sender is FrameworkElement element)
            {
                var process = ((KeyValuePair<int, Process>)element.DataContext).Value;

                // Bind default process (store for other page to pick up).
                IoC.Kernel.Unbind<Process>();
                IoC.Kernel.Bind<Process>().ToConstant(process);
            }

            // Set the sidebar current game for page switch and re-set binding for new page.
            var viewModel = IoC.Get<SidebarViewModel>();
            viewModel.Page = Page.Process;
        }

        private void ControlsButtonA_Click(object sender, MouseButtonEventArgs e)
        {
            // Set the sidebar current game for page switch and re-set binding for new page.
            var viewModel = IoC.Get<SidebarViewModel>();
            viewModel.Page = Page.ControlsA;
        }

        private void ControlsButtonB_Click(object sender, MouseButtonEventArgs e)
        {
            // Set the sidebar current game for page switch and re-set binding for new page.
            var viewModel = IoC.Get<SidebarViewModel>();
            viewModel.Page = Page.ControlsB;
        }
    }
}
