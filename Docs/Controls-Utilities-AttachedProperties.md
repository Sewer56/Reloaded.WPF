<div align="center">
	<h1>Reloaded.WPF: Controls, Utilities & Attached Properties</h1>
	<img src="https://i.imgur.com/BjPn7rU.png" width="150" align="center" />
	<br/> <br/>
	<strong><i>You expected cringeworthy text, but <strike>nobody</strike> nothing came!</i></strong>
</div>

## Page Information

🕒  Reading Time: 10 Minutes

## Table of Contents
- [Controls](#controls)
  - [Page Host / Page Switcher (PageHost)](#page-host-page-switcher-pagehost)
  - [Circular Button (CircleButton)](#circular-button-circlebutton)
- [Utilities](#utilities)
  - [Binding Proxy (BindingProxy)](#binding-proxy-bindingproxy)
  - [Attached Property Base (AttachedPropertyBase)](#attached-property-base-attachedpropertybase)
  - [Resource Manipulator (ResourceManipulator)](#resource-manipulator-resourcemanipulator)
  - [Action Command (ActionCommand)](#action-command-actioncommand)
  - [Observable Object (ObservableObject)](#observable-object-observableobject)
- [Attached Properties](#attached-properties)
  - [Disable Frame History (EnableFrameHistory)](#disable-frame-history-enableframehistory)

## Controls

All custom controls exposed can be found in the `Reloaded.WPF.Controls` namespace. 

### Page Host / Page Switcher (PageHost)
The page switcher control is what allows you to switch between multiple pages with animation support for pages which inherit from the `PageBase` class (this includes `ReloadedPage`).

![Page Switching Animation](https://camo.githubusercontent.com/14685b8d5b5f1b43e35ff11547522052f09a4321/68747470733a2f2f692e696d6775722e636f6d2f4e42626e69426f2e676966)

Using the page switcher is rather simple, there is a a singular property `CurrentPage`. If the `CurrentPage` property value changes, the old page is animated out and the new page is animated in. 

**Usage**
```xml
<controls:PageHost ClipToBounds="True" CurrentPage="{Binding Page}"/>
```
### Circular Button (CircleButton)

The `CircleButton` is a very simple control that hosts an image that is cropped/clipped to the shape of a circle, perfect for hosting small or large icons/pictures.

![Sewer56 Logo in CircleButton](https://i.imgur.com/R0wtrcT.png)

*Reloaded author's logo hosted in a CircleButton*.

There are two properties `ImageSource`, which is the image to show and `TooltipText`, which is the text to display in a tooltip when the user hovers over the button.

The tooltip is not forced, and as such if no tooltip text is assigned, a tooltip will not show.

## Utilities

### Binding Proxy (BindingProxy)
The `BindingProxy` (inside `Reloaded.WPF.Controls`) is a `Freezable` `DependencyObject` of the WPF framework that does only one thing: Store arbitrary data that can be binded to.

This allows us to use this class as a proxy to bind to arbitrary elements which may or may not be explicitly be part of the WPF framework, while still being able to use the full power of WPF's binding system such as IValueConverters.

To use this class, simply make a key(ed) instance of this class in the resources of any WPF element
such as a window or control.

```xml
<Window.Resources>
	<local:BindingProxy x:Key"proxy" Data="{DynamicResource resource}" />
<Window.Resources>
```

In this case above, we store a `DynamicResource` in the place of the data of the proxy.

What can we do with this? Here's an idea or two:

- Bind to a Color with a from a ResourceDictionary, with a ValueConverter that creates a SolidColorBrush. No need to create a separate brush for every color.
- Use multiple ViewModels inside one page, keeping all binding code inside XAML.

### Attached Property Base (AttachedPropertyBase)

`Reloaded.WPF.MVVM` contains a simple base class for defining attached properties: `AttachedPropertyBase<TParent, TProperty>`.

It allows for the creation of simple attached properties consisting of only one value property "Value" by inheriting the `AttachedPropertyBase` class, which automatically exposes events for when the value is changed (to a different value) or updated (same or different value). 

**Example:**
```
public class SomeAttachedProperty : AttachedPropertyBase<SomeAttachedProperty, bool>
{
    public override void OnValueUpdated(DependencyObject sender, object value)
    {
	    /* Do something. */
	    // sender: WPF Framework object
	    // value: New value of property. (bool)
    }
}
```

For an actual functional example, refer to the *NoFrameHistory* attached property class.

### Resource Manipulator (ResourceManipulator)

The `ResourceManipulator` inside `Reloaded.WPF.Utilities` is a simple utility class that allows for easy access of XAML Resources (ResourceDictionary) from code-behind.

**Interface**
Actual documentation omitted to keep example concise.
```csharp
// Constructor
public ResourceManipulator(FrameworkElement element) { _element = element; }

// Gets a resource from the resource dictionary of the window.
TResource Get<TResource>(string resourceName);

// Sets the value of a resource in the resource dictionary of the window.
void Set<TResource>(string resourceName, TResource value);
```

### XAML Resource (XamlResource)
The `XamlResource` inside `Reloaded.WPF.Utilities` is a brother of the `ResourceManipulator` class, allowing for easy acess of XAML Resources from code-behind.

**Interface**
Actual documentation omitted to keep example concise.
```csharp
// Make XAML Resource
var XamlEntrySlideAnimationDuration = new XamlResource<double>("EntrySlideAnimationDuration");

// Get a resource.
XamlEntrySlideAnimationDuration.Get();

// Set a resource
XamlEntrySlideAnimationDuration.Set(value);
```

#### Behaviour
By default, this class will look inside the application resources `Application.Current.Resources` for the XAML element. 

Additional sources can be specified using either overloads of the constructor or by changing the `AdditionalSources` property.

For Get/Set, the class attempts to do these operations in the following order: 

- 1. Application
- 2. `AdditionalSources`

Until it finds the first element that contains the key, and exits looking in no further elements.

If you would like to edit which source gets searched first, there also exists a `BiasedElement` property.


### Action Command (ActionCommand)

The most common, famous, well known implementation of `ICommand` that accepts an `Action` (parameterless function or method) to execute some arbitrary code when attached to a framework element such as a button.

```csharp
// Assuming "this" is a window.
MinimizeCommand = new ActionCommand(() => { this.WindowState = WindowState.Minimized; });
```

### Observable Object (ObservableObject)

The most common, famous, simple class featuring nothing but an implementation of `INotifyPropertyChanged`.
Intended to be inherited by other classes and optionally (but recommended) used in conjunction with [PropertyChanged.Fody](https://github.com/Fody/PropertyChanged) to automatically inject calls to `PropertyChanged`.

To make use in this class, add `PropertyChanged.Fody` manually as a NuGet package to your own project.

## Attached Properties

Here is a list of pre-included attached properties in Reloaded.WPF, found in `Reloaded.MVVM.Properties`.

### Disable Frame History (EnableFrameHistory)

`EnableFrameHistory` is a simple WPF attached property that allows a WPF `Frame` control to keep an empty navigation history and therefore not show the navigation bar onscreen.

**Usage**
```
<Frame EnableFrameHistory.Value="False"/>
```
