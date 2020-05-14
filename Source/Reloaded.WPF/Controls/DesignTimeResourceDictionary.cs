using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Reloaded.WPF.Controls
{
    /// <summary>
    /// A modified type of <see cref="ResourceDictionary"/> that allows for having different runtime and design time
    /// locations. Allows for better support of theming at design-time.
    /// </summary>
    public class DesignTimeResourceDictionary : ResourceDictionary, INotifyPropertyChanged
    {
        private static readonly string UseRedirectSource = $"Please use {nameof(DesignTimeSource)} and {nameof(RunTimeSource)} instead of Source";

        /// <summary>
        /// Source to be executed at design time.
        /// </summary>
        public string DesignTimeSource { get; set; }

        /// <summary>
        /// Source to be executed at runtime.
        /// </summary>
        public string RunTimeSource    { get; set; }

        /// <summary>
        /// Resource dictionary that allows for assigning to different resource at design time.
        /// </summary>
        public DesignTimeResourceDictionary()
        {
            this.PropertyChanged += OnPropertyChanged;
            UpdateSource();
        }

        /// <summary>
        /// Update source on edit.
        /// </summary>
        private void UpdateSource()
        {
            bool isInDesignMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

            if (isInDesignMode)
            {
                if (DesignTimeSource != null)
                    base.Source = new Uri(DesignTimeSource, UriKind.RelativeOrAbsolute);
            }
            else
            {
                if (RunTimeSource != null)
                    base.Source = new Uri(RunTimeSource, UriKind.RelativeOrAbsolute);
            }
        }

        /// <summary>
        /// Auto update dictionary on chosen XAML file.
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DesignTimeSource) || e.PropertyName == nameof(RunTimeSource))
                UpdateSource();
        }

        public new Uri Source
        {
            get => throw new Exception(UseRedirectSource);
            set => throw new Exception(UseRedirectSource);
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
