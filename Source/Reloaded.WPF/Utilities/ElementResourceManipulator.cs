using System.Windows;

namespace Reloaded.WPF.Utilities
{
    /// <summary>
    /// A helper class which lets you access the XAML Resources
    /// of a WPF framework element.
    /// </summary>
    public class ElementResourceManipulator
    {
        private FrameworkElement _element;

        /// <summary/>
        /// <param name="element">The WPF framework element (e.g. Window, Control).</param>
        public ElementResourceManipulator(FrameworkElement element)
        {
            _element = element;
        }

        /// <summary>
        /// Gets a resource from the resource dictionary of the window.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>The value of the resource.</returns>
        public TResource Get<TResource>(string resourceName)
        {
            return _element.Dispatcher.Invoke
            (
                () => (TResource)_element.Resources[resourceName]
            );
        }

        /// <summary>
        /// Sets the value of a resource in the resource dictionary of the window.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="value">The new value of the resource.</param>
        public void Set<TResource>(string resourceName, TResource value)
        {
            _element.Dispatcher.Invoke
            (
                () => { _element.Resources[resourceName] = value; }
            );
        }
    }
}
