using System;
using System.Collections.Concurrent;
using System.Management;
using System.Windows;
using PropertyChanged;
using Reloaded.WPF.MVVM;
using Reloaded.WPF.TestWindow.Models.Model;
using Reloaded.WPF.TestWindow.Pages;
using Reloaded.WPF.TestWindow.Utilities;

namespace Reloaded.WPF.TestWindow.Models.ViewModel
{
    public class SidebarViewModel : ObservableObject
    {
        /// <summary>
        /// A list of all process models available on the sidebar.
        /// </summary>
        public ObservableConcurrentDictionary<int, Process> ProcessModels { get; set; } = new ObservableConcurrentDictionary<int, Process>();

        /// <summary>
        /// The currently displayed page on this window.
        /// </summary>
        [DoNotCheckEquality]
        public Pages.Page Page { get; set; }

        private ProcessWatcher _watcher;

        public SidebarViewModel()
        {
            // Populate bindings.
            Page = Page.None;
            PopulateProcesses();
            _watcher = new ProcessWatcher();
            _watcher.OnNewProcess += OnNewProcess;
            _watcher.OnRemovedProcess += OnRemovedProcess;
        }

        private void OnRemovedProcess(int processId) => ProcessModels.Remove(processId);
        private void OnNewProcess(System.Diagnostics.Process newProcess) => AddProcess(newProcess);

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
    }
}
