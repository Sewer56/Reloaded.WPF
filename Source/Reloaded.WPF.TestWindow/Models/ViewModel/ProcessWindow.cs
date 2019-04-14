using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reloaded.WPF.TestWindow.Models.Model;
using Reloaded.WPF.Theme.Default;

namespace Reloaded.WPF.TestWindow.Models.ViewModel
{
    /// <summary>
    /// A simple viewmodel that stores the currently selected process in the GUI.
    /// </summary>
    public class ProcessWindow
    {       
        /// <summary>
        /// The currently selected UI process.
        /// </summary>
        public Process Process { get; set; }

        /// <summary>
        /// Access to the busy property of the window.
        /// </summary>
        public WindowViewModel WindowViewModel { get; }

        public ProcessWindow(Process process)
        {
            Process = process;
            WindowViewModel = IoC.Get<WindowViewModel>();
        }
    }
}
