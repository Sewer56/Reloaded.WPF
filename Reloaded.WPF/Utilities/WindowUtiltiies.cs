using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Reloaded.WPF.Utilities
{
    public static class WindowUtilities
    {
        /// <summary>
        /// Adds the ResourceDictionary applied to the current window to the content of the child frame.
        /// </summary>
        /// <param name="sender">The frame to be styled.</param>
        public static void SyncResourceDictionary(object sender, EventArgs e)
        {
            Frame frame = sender as Frame;
            FrameworkElement content = frame.Content as FrameworkElement;

            // Find Window element.
            var windowElement = FindParent<Window>(frame);

            if (windowElement != null)
                content.Resources.MergedDictionaries.Add(windowElement.Resources);
        }

        /// <summary>
        /// Finds a parent to a given <see cref="DependencyObject"/> (e.g. <see cref="FrameworkElement"/>) of a specified type.
        /// </summary>
        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null)
                return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}
