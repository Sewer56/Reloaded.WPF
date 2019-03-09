using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PropertyChanged;

namespace Reloaded.WPF.ViewModels.Base
{
    [AddINotifyPropertyChangedInterface]
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        
        /// <summary>
        /// Calls the PropertyChange event manually.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
