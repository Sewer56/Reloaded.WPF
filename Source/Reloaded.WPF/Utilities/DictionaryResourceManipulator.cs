using System.Windows;

namespace Reloaded.WPF.Utilities
{
    /// <summary>
    /// A helper class which lets you access the XAML Resources
    /// of a ResourceDictionary.
    /// </summary>
    public class DictionaryResourceManipulator
    {
        private ResourceDictionary _dictionary;

        /// <summary/>
        public DictionaryResourceManipulator(ResourceDictionary dictionary)
        {
            _dictionary = dictionary;
        }

        /// <summary>
        /// Gets a resource from the resource dictionary.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>The value of the resource.</returns>
        public TResource Get<TResource>(string resourceName)
        {
            return (TResource) _dictionary[resourceName];
        }

        /// <summary>
        /// Sets the value of a resource in the resource dictionary.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="value">The new value of the resource.</param>
        public void Set<TResource>(string resourceName, TResource value)
        {
            _dictionary[resourceName] = value;
        }
    }
}
