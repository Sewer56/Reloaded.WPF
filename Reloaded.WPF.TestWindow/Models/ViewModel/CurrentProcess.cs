using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reloaded.WPF.TestWindow.Models.Model;

namespace Reloaded.WPF.TestWindow.Models.ViewModel
{
    /// <summary>
    /// A simple viewmodel that stores the currently selected process in the GUI.
    /// </summary>
    public class CurrentProcess
    {       
        /// <summary>
        /// The currently selected UI process.
        /// </summary>
        public Process Process { get; set; }

        public CurrentProcess(Process process)
        {
            Process = process;
        }
    }
}
