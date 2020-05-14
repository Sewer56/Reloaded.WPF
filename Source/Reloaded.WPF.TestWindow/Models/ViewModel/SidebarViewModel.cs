using System;
using System.Collections.Concurrent;
using System.Management;
using System.Windows;
using PropertyChanged;
using Reloaded.WPF.MVVM;
using Reloaded.WPF.TestWindow.Models.Model;
using Reloaded.WPF.TestWindow.Pages;

namespace Reloaded.WPF.TestWindow.Models.ViewModel
{
    public class SidebarViewModel : ObservableObject
    {
        /// <summary>
        /// The name of the Process ID WQL column in Windows Management Instrumentation.
        /// </summary>
        private const string WMI_PROCESSID_NAME = "ProcessID";

        /// <summary>
        /// A list of all process models available on the sidebar.
        /// </summary>
        public ObservableConcurrentDictionary<int, Process> ProcessModels { get; set; } = new ObservableConcurrentDictionary<int, Process>();

        /// <summary>
        /// The currently displayed page on this window.
        /// </summary>
        [DoNotCheckEquality]
        public Pages.Page Page { get; set; }

        private ManagementEventWatcher _startWatcher;
        private ManagementEventWatcher _stopWatcher;

        public SidebarViewModel()
        {
            // Populate bindings.
            Page = Page.None;
            PopulateProcesses();
            _startWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            _stopWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            _startWatcher.EventArrived += ApplicationLaunched;
            _stopWatcher.EventArrived += ApplicationExited;
        }

        /// <summary>
        /// Initially populates a list of processes.
        /// </summary>
        private void PopulateProcesses()
        {
            foreach (int key in ProcessModels.Keys)
                ProcessModels.Remove(key);

            foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses())
                AddProcess(process);
        }

        /// <summary>
        /// Adds a process to the ObservableCollection of Processes.
        /// </summary>
        private void AddProcess(System.Diagnostics.Process process)
        {
            var model = new Process(process);
            Application.Current.Dispatcher.Invoke(() => {
                ProcessModels.Add(process.Id, model);
            });
        }

        /// <summary>
        /// Adds a process to the ObservableCollection when it is launched.
        /// </summary>
        private void ApplicationLaunched(object sender, EventArrivedEventArgs e)
        {
            try
            {
                AddProcess(System.Diagnostics.Process.GetProcessById((int)e.NewEvent.Properties[WMI_PROCESSID_NAME].Value));
            }
            catch (Exception) { /* ignored */ }
        }

        /// <summary>
        /// Executed when one of the applications exits.
        /// </summary>
        private void ApplicationExited(object sender, EventArrivedEventArgs e)
        {
            ProcessModels.Remove((int)e.NewEvent.Properties[WMI_PROCESSID_NAME].Value);
        }
    }
}
