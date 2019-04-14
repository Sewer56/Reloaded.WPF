using System.ComponentModel;

namespace Reloaded.WPF.MVVM
{
    /// <summary>
    /// An abstract class that implements the bare minimum of the INotifyPropertyChanged interface.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
