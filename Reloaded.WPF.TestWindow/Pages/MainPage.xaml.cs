using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using ORMi;
using Reloaded.WPF.TestWindow.Models;
using Reloaded.WPF.Theme.Default;
using Reloaded.WPF.Utilities;

namespace Reloaded.WPF.TestWindow.Pages
{
    /// <summary>
    /// The main page of the application.
    /// </summary>
    public partial class MainPage : System.Windows.Controls.Page
    {
        /// <summary>
        /// The currently displayed page on this window.
        /// </summary>
        public Pages.Page GamePage { get; set; } = Page.Game;

        /// <summary>
        /// A list of all process models available on the sidebar.
        /// </summary>
        public ObservableConcurrentDictionary<int, ProcessModel> ProcessModels { get; set; } = new ObservableConcurrentDictionary<int, ProcessModel>();

        private WMIWatcher _startWatcher;
        private WMIWatcher _stopWatcher;

        public MainPage()
        {  
            InitializeComponent();
            Loader.Load(this);

            // Populate bindings.
            PopulateProcesses();
            _startWatcher = new WMIWatcher("root\\CimV2", "SELECT * FROM Win32_ProcessStartTrace", typeof(WMIProcess));
            _stopWatcher = new WMIWatcher("root\\CimV2", "SELECT * FROM Win32_ProcessStopTrace", typeof(WMIProcess));
            _startWatcher.WMIEventArrived += ApplicationLaunched;
            _stopWatcher.WMIEventArrived += ApplicationExited;
        }

        /// <summary>
        /// Initially populates a list of processes.
        /// </summary>
        private void PopulateProcesses()
        {
            foreach (int key in ProcessModels.Keys)
                ProcessModels.Remove(key);

            foreach (Process process in Process.GetProcesses())
                AddProcess(process);
        }

        /// <summary>
        /// Adds a process to the ObservableCollection of Processes.
        /// </summary>
        private void AddProcess(Process process)
        {
            var model = new ProcessModel(process);
            this.ProcessesPanel.Dispatcher.Invoke(() => {
                ProcessModels.Add(process.Id, model);
            });
        }

        /// <summary>
        /// Adds a process to the ObservableCollection when it is launched.
        /// </summary>
        private void ApplicationLaunched(object sender, WMIEventArgs e)
        {
            WMIProcess process = (WMIProcess)e.Object;
            try
            {
                AddProcess(Process.GetProcessById(process.ProcessID));
            }
            catch (Exception) { /* ignored */ }
        }

        /// <summary>
        /// Executed when one of the applications exits.
        /// </summary>
        private void ApplicationExited(object sender, WMIEventArgs e)
        {
            WMIProcess process = (WMIProcess)e.Object;
            ProcessModels.Remove(process.ProcessID);
        }

        /// <summary>
        /// Represents a singular instance of a process as defined by the Windows Management Instrumentation.
        /// </summary>
        [WMIClass("Win32_ProcessStartTrace")]
        public class WMIProcess
        {
            public string ProcessName { get; set; }
            public int ProcessID { get; set; }
        }
    }
}
