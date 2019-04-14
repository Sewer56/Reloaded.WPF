using System;
using System.Windows;

namespace Reloaded.WPF.MVVM
{
    /// <summary>
    /// A base class that allows for the easier creation of WPF "Attached Properties".
    /// Inheriting this class allows you to 
    /// </summary>
    /// <typeparam name="TParent">The parent class to become the attached property itself, i.e. the class that inherits this base.</typeparam>
    /// <typeparam name="TProperty">The data type of the individual attached property e.g. "bool". </typeparam>
    public abstract class AttachedPropertyBase<TParent, TProperty> where TParent : new()
    {
        public static TParent Instance { get; private set; } = new TParent();

        /// <summary>
        /// Registers the actual dependency property itself to be visible from WPF and XAML.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(TProperty),
            typeof(AttachedPropertyBase<TParent, TProperty>),
            new UIPropertyMetadata(default(TProperty), OnValuePropertyChanged, OnValuePropertyUpdated));

        /// <summary>
        /// This event is executed when the value of the attached property changes.
        /// e.g. The value becomes false after being true.
        /// </summary>
        public event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (sender, e) => { };

        /// <summary>
        /// This event is fired when the value is set, regardless of whether the value is the same or not.
        /// </summary>
        public event Action<DependencyObject, object> ValueUpdated = (sender, value) => { };

        /// <summary>
        /// Executed when the <see cref="ValueProperty"/> is changed to another value.
        /// i.e. When "false" becomed "true".
        /// </summary>
        /// <param name="dependencyObject">The WPF framework element of whose property was changed.</param>
        /// <param name="changedEventArgs">The arguments for the event.</param>
        private static void OnValuePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs changedEventArgs)
        {
            // Call both the event and the virtual method (inherited by parent).
            var attachedProperty = Instance as AttachedPropertyBase<TParent, TProperty>;
            attachedProperty?.OnValueChanged(dependencyObject, changedEventArgs);
            attachedProperty?.ValueChanged(dependencyObject, changedEventArgs);
        }

        /// <summary>
        /// The callback event when the <see cref="ValueProperty"/> is set, regardless of whether
        /// it is the same value or not.
        /// </summary>
        /// <param name="dependencyObject">The WPF framework element of whose property was changed.</param>
        /// <param name="value">The new value to be set.</param>
        private static object OnValuePropertyUpdated(DependencyObject dependencyObject, object value)
        {
            var attachedProperty = Instance as AttachedPropertyBase<TParent, TProperty>;
            attachedProperty?.OnValueUpdated(dependencyObject, value);
            attachedProperty?.ValueUpdated(dependencyObject, value);

            return value;
        }

        /// <summary>
        /// Retrieves the value of the attached property for a specified <see cref="DependencyObject"/>.
        /// </summary>
        public static TProperty GetValue(DependencyObject dependencyObject) => (TProperty)dependencyObject.GetValue(ValueProperty);

        /// <summary>
        /// Sets the value of the attached property for a specified <see cref="DependencyObject"/>.
        /// </summary>
        public static void SetValue(DependencyObject dependencyObject, TProperty value) => dependencyObject.SetValue(ValueProperty, value);

        /// <summary>
        /// See <see cref="OnValuePropertyChanged"/>.
        /// </summary>
        public virtual void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        /// <summary>
        /// See <see cref="OnValuePropertyUpdated"/>.
        /// </summary>
        public virtual void OnValueUpdated(DependencyObject sender, object value) { }
    }
}