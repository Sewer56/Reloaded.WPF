using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Reloaded.WPF.MVVM
{
    /// <summary>
    /// An abstract <see cref="DependencyObject"/> class that implements the bare minimum of the INotifyPropertyChanged interface.
    /// </summary>
    public class ObservableDependencyObject : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
