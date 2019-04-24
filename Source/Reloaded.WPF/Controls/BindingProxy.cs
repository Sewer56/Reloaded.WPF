using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Reloaded.WPF.Controls
{
    /// <summary>
    /// The <see cref="BindingProxy"/> is a <see cref="Freezable"/> <see cref="DependencyObject"/> of the WPF
    /// framework that does only one thing: Store arbitrary data that can be binded to.
    /// 
    /// This allows us to use this class as a proxy to bind to arbitrary elements
    /// which may or may not be explicitly be part of the WPF framework, while still 
    /// being able to use the full power of WPF's binding system such as 
    /// <see cref="IValueConverter"/>s.
    ///
    /// To use this class, simply make a key(ed) instance of this class in the resources of any WPF element
    /// such as a window or control.
    ///
    /// e.g.
    /// &lt;Window.Resources&gt;
    ///     &lt;local:BindingProxy x:Key="proxy" Data="{Binding}" /&gt;
    /// &lt;/Window.Resources&gt; 
    /// </summary>
    /// <remarks>
    /// Inspired/taken from: https://thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/
    /// </remarks>
    public class BindingProxy : Freezable
    {
        // Use DependencyProperty as the backing store for Data.
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}
