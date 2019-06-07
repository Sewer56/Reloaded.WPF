using System;
using System.ComponentModel;
using System.Windows;

namespace Reloaded.WPF.Controls
{
    /// <summary>
    /// A modified type of <see cref="ResourceDictionary"/> that translates "Resource" pack URIs to "Site of Origin"
    /// pack URIs when in runtime.
    ///
    /// i.e. This allows you to declare pack URIs as "pack://application:,,,", which will be resolved
    /// as such in design mode while at runtime, actually using "pack://siteoforigin:,,,".
    /// </summary>
    public class SiteOfOriginResourceDictionary : ResourceDictionary
    {
        private const string SiteOfOriginPrefix = "pack://siteoforigin:,,,";
        private const string ApplicationPrefix = "pack://application:,,,";

        private const string UseRedirectSource = "Please use RedirectSource instead of Source";
        private string _originalUri;

        /// <summary>
        /// Gets or sets the design time source.
        /// </summary>
        public string RedirectSource
        {
            get => _originalUri;

            set
            {
                this._originalUri = value;
                bool isInDesignMode = (bool) DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

                if (! isInDesignMode)
                {
                    if (value.Contains(ApplicationPrefix))
                        value = value.Replace(ApplicationPrefix, SiteOfOriginPrefix);
                }

                base.Source = new Uri(value);
            }
        }

        /// <summary>
        /// Please use <see cref="RedirectSource"/>
        /// </summary>
        public new Uri Source
        {
            get => throw new Exception(UseRedirectSource);
            set => throw new Exception(UseRedirectSource);
        }
    }
}
