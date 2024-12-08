#pragma warning disable 1591

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Reloaded.WPF.MVVM.Properties
{
    /// <summary>
    /// Allows to create a <see cref="Frame"/> that keeps an empty navigation history and does not
    /// show the navigation bar onscreen.
    /// </summary>
    public class EnableFrameHistory : AttachedPropertyBase<EnableFrameHistory, bool>
    {
        public override void OnValueUpdated(DependencyObject sender, object value)
        {
            if (sender is Frame frame)
            {
                if ((bool)value == false) { DisableHistory(frame); }
                else { EnableHistory(frame); }
            }
        }

        private void DisableHistory(Frame frame)
        {
            frame.Navigated += RemoveBackEntry;
            frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }

        private void EnableHistory(Frame frame)
        {
            frame.Navigated -= RemoveBackEntry;
            frame.NavigationUIVisibility = NavigationUIVisibility.Visible;
        }

        private void RemoveBackEntry(object sender, object eventArgs)
        {
            if (sender is Frame frame)
            {
                try
                {
                    frame.NavigationService.RemoveBackEntry();
                }
                catch (Exception) { /* Ignored */ }
            }
        }
    }
}