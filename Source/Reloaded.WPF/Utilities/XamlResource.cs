using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Reloaded.WPF.Utilities
{
    /// <summary>
    /// Class that provides easy and convenient access to XAML resources loaded in through <see cref="ResourceDictionary"/>-ies.
    /// </summary>
    public class XamlResource<TResourceType>
    {
        /// <summary>
        /// Additional sources in which the resource should be searched in.
        /// </summary>
        public List<FrameworkElement> AdditionalSources { get; set; }

        /// <summary>
        /// Defines the first element to search in Get/Set operations.
        /// </summary>
        public FrameworkElement BiasedElement { get; set; }

        /// <summary>
        /// The value to return by default if <see cref="Get"/> fails.
        /// </summary>
        public TResourceType DefaultValue { get; set; }

        private object _resourceKey;

        /// <summary>
        /// Creates a <see cref="XamlResource{TResourceType}"/> given the key of the object to search.
        /// </summary>
        /// <param name="resourceKey">The key of the object to search.</param>
        public XamlResource(object resourceKey)
        {
            _resourceKey = resourceKey;
            AdditionalSources = new List<FrameworkElement>();
        }

        /// <summary>
        /// Creates a <see cref="XamlResource{TResourceType}"/> given the key of the object to search and a collection of additional sources to search in.
        /// </summary>
        /// <param name="resourceKey">The key of the object to search.</param>
        /// <param name="additionalSources">Additional sources to search the object in.</param>
        public XamlResource(object resourceKey, IEnumerable<FrameworkElement> additionalSources)
        {
            _resourceKey = resourceKey;
            AdditionalSources = additionalSources.ToList();
        }

        /// <summary>
        /// Creates a <see cref="XamlResource{TResourceType}"/> given the key of the object to search, the element to search first
        /// and a collection of additional sources to search in.
        /// </summary>
        /// <param name="resourceKey">The key of the object to search.</param>
        /// <param name="additionalSources">Additional sources to search the object in.</param>
        /// <param name="biasedElement">The first element to search.</param>
        public XamlResource(object resourceKey, IEnumerable<FrameworkElement> additionalSources, FrameworkElement biasedElement)
        {
            _resourceKey = resourceKey;
            AdditionalSources = additionalSources.ToList();
            BiasedElement = biasedElement;
        }

        /// <summary>
        /// Searches the Application Resources as well as all <see cref="AdditionalSources"/> for the element with the key.
        /// </summary>
        /// <returns>The value set in the XAML resources, or a default value.</returns>
        public TResourceType Get()
        {
            // Try biased source.
            if (BiasedElement != null)
                if (BiasedElement.Resources.Contains(_resourceKey))
                    return GetFromElement(BiasedElement);

            // First try application.
            if (Application.Current != null)
                if (Application.Current.Resources.Contains(_resourceKey))
                    return GetFromApplication();

            // Now try all sources.
            foreach (var source in AdditionalSources)
            {
                if (source.Resources.Contains(_resourceKey))
                    return GetFromElement(source);
            }

            if (DefaultValue != null)
                return DefaultValue;

            // Failed
            return default;
        }

        /// <summary>
        /// Sets the value to the first Application Resource or first of <see cref="AdditionalSources"/> containing an entry for the key.
        /// </summary>
        /// <param name="value">The value to assign to the resource.</param>
        /// <returns>True if the operation succeeded else false./</returns>
        public bool Set(TResourceType value)
        {
            // Try biased source.
            if (BiasedElement != null)
            {
                if (BiasedElement.Resources.Contains(_resourceKey))
                {
                    SetToElement(BiasedElement, value);
                    return true;
                }
            }

            // Try Application
            if (Application.Current != null)
            {
                if (Application.Current.Resources.Contains(_resourceKey))
                {
                    SetToApplication(value);
                    return true;
                }
            }

            // Now try all sources.
            foreach (var source in AdditionalSources)
            {
                if (source.Resources.Contains(_resourceKey))
                {
                    SetToElement(source, value);
                    return true;
                }
            }

            return false;
        }

        /* Dispatchers */
        private TResourceType GetFromApplication()
        {
            return Application.Current.Dispatcher.Invoke
            (
                () => (TResourceType)Application.Current.Resources[_resourceKey]
            );
        }

        private void SetToApplication(TResourceType value)
        {
            Application.Current.Dispatcher.Invoke
            (
                () => { Application.Current.Resources[_resourceKey] = value; }
            );
        }

        private TResourceType GetFromElement(FrameworkElement element)
        {
            return element.Dispatcher.Invoke
            (
                () => (TResourceType)element.Resources[_resourceKey]
            );
        }

        private void SetToElement(FrameworkElement element, TResourceType value)
        {
            element.Dispatcher.Invoke
            (
                () => { element.Resources[_resourceKey] = value; }
            );
        }
    }
}
