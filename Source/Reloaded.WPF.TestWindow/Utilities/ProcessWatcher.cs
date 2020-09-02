using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using Reloaded.WPF.MVVM;

namespace Reloaded.WPF.TestWindow.Utilities
{
    /// <summary>
    /// Utility class that provides events for when processes start up and/or shut down without using WMI.
    /// More resource wasteful and has higher latency but does not require administrative privileges.
    /// </summary>
    public class ProcessWatcher : ObservableObject, IDisposable
    {
        public static ProcessWatcher Instance { get; } = new ProcessWatcher();

        public event ProcessArrived OnNewProcess    = process => { };
        public event ProcessExited OnRemovedProcess = processId => { };

        private Timer _timer;
        private ObservableCollection<int> _processes;
        private object _lock = new object();

        public ProcessWatcher()
        {
            _processes = new ObservableCollection<int>(ProcessExtensions.GetProcessIds());
            _processes.CollectionChanged += ProcessesChanged;
            _timer = new Timer(Tick, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(3000));
        }

        private void Tick(object state)
        {
            lock (_lock)
            {
                var processIds = ProcessExtensions.GetProcessIds();
                Collections.ModifyObservableCollection(_processes, processIds);
            }
        }

        private void ProcessesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            try { OnNewProcess(Process.GetProcessById((int)newItem)); }
                            catch (Exception) { }
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var oldItem in e.OldItems)
                            OnRemovedProcess((int)oldItem);
                        break;
                    }
            }
        }

        public void Dispose() { }
    }

    public delegate void ProcessArrived(Process newProcess);
    public delegate void ProcessExited(int processId);
}
