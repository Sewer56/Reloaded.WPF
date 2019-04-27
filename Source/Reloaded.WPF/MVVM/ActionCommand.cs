#pragma warning disable 1591

using System;
using System.Windows.Input;

namespace Reloaded.WPF.MVVM
{
    /// <summary>
    /// Simple WPF command that executes an <see cref="Action"/>.
    /// </summary>
    public class ActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged = (sender, e) => { };
        private Action _action;

        public ActionCommand(Action action)
        {
            _action = action;
        }
        
        public bool CanExecute(object parameter)
        {
            // A delegate command can always execute
            return true;
        }
        
        public void Execute(object parameter)
        {
            // Executes the delegate.
            _action();
        }
    }
}