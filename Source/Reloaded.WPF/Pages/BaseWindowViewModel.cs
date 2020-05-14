#pragma warning disable 1591

using System;
using System.Windows;
using System.Windows.Input;
using Reloaded.WPF.MVVM;
using Reloaded.WPF.Utilities;

namespace Reloaded.WPF.Pages
{
    public class BaseWindowViewModel : ObservableDependencyObject
    {
        public event Action<WindowDockPosition>  WindowDockChanged = position => { };

        /* Default Minimize, Maximize, Close implementations. */
        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand    { get; set; }

        /* Local variables. */
        protected System.Windows.Window TargetWindow;
        protected WindowResizer       WindowResizer;
        protected WindowDockPosition  DockPosition = WindowDockPosition.Undocked;

        /// <summary>
        /// Constructor for the base view model.
        /// </summary>
        public BaseWindowViewModel(System.Windows.Window window)
        {
            // Note that we are breaking MVVM concepts by accepting this parameter,
            // though this is not a viewmodel we will probably use in non-desktop
            // environments and everything here is specific to desktop style.
            TargetWindow   = window;
            WindowResizer  = new WindowResizer(TargetWindow);

            /* Setup hooks for dock position change. */
            WindowResizer.WindowDockChanged += WindowDockChanged;

            // Implement Titlebar Buttons
            MinimizeCommand = new ActionCommand(() => { TargetWindow.WindowState = WindowState.Minimized; });
            MaximizeCommand = new ActionCommand(() => { TargetWindow.WindowState ^= WindowState.Maximized; });
            CloseCommand = new ActionCommand(() =>    { TargetWindow.Close(); } );
        }
    }
}
