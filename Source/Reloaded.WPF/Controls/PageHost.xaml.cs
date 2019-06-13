﻿#pragma warning disable 1591

using System;
using System.Windows;
using System.Windows.Controls;
using Reloaded.WPF.Pages;

namespace Reloaded.WPF.Controls
{
    /// <summary>
    /// The <see cref="PageHost"/> is a class that allows for navigation between <see cref="Frame"/>s that inherit
    /// from the <see cref="PageBase"/> class. It allows for the automatic animating in of new pages and animating out of
    /// old pages.
    /// </summary>
    public partial class PageHost : UserControl
    {
        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register(nameof(CurrentPage), typeof(PageBase), typeof(PageHost), new PropertyMetadata(default(PageBase), PropertyChangedCallback, PropertyChanged));

        public PageBase CurrentPage
        {
            get => (PageBase)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public PageHost()
        {
            InitializeComponent();
        }

#pragma warning disable 4014
        /// <summary>
        /// Called when the value of the <see cref="CurrentPageProperty"/> is changed, i.e. new page is set.
        /// </summary>
        /// <param name="dependencyObject">The object the property is attached to, i.e. 'this'/<see cref="PageHost"/> in our case. </param>
        /// <param name="newValue">The new value to be set.</param>
        private static object PropertyChanged(DependencyObject dependencyObject, object newValue)
        {
            if (dependencyObject is PageHost switcher)
            {
                // Transfer the current page to the secondary frame.
                switcher.OldPage.Content = switcher.NewPage.Content;
                switcher.NewPage.Content = newValue;

                void AnimateOutHandler(object sender, EventArgs args)
                {
                    if (switcher.OldPage.Content is PageBase oldPage)
                        oldPage.AnimateOut();

                    switcher.OldPage.ContentRendered -= AnimateOutHandler;
                }

                void AnimateInHandler(object sender, EventArgs args)
                {
                    if (switcher.NewPage.Content is PageBase newPage)
                        newPage.AnimateIn();

                    switcher.NewPage.ContentRendered -= AnimateInHandler;
                }

                switcher.OldPage.ContentRendered += AnimateOutHandler;
                switcher.NewPage.ContentRendered += AnimateInHandler;
            }

            return newValue;
        }
#pragma warning restore 4014

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            /* Not Implemented */
        }
    }
}
