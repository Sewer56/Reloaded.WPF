<div align="center">
	<h1>Reloaded.WPF: Getting Started</h1>
	<img src="https://i.imgur.com/BjPn7rU.png" width="150" align="center" />
	<br/> <br/>
	<strong><i>You expected a complicated guide, but it was I! Getting Started!</i></strong>
</div>

## Page Information

🕒  Reading Time: 05-10 Minutes

## Introduction

The following is a small, quick, non-exhaustive resource to help you get started with the *Reloaded.WPF* library - providing an introduction to using the library. This serves as a guide to help you get going, covering the basics and essentials.

## Pre-Prologue: Adding Reloaded.WPF to your project.
1.  Open/Create project in Visual Studio.
2.  Right-click your project within the `Solution Explorer` and select `Manage NuGet Packages`.
3.  Search for `Reloaded.WPF.Theme.Default`.
4.  Install the package.

Note: The default theme is not compulsory, you may simply use the core `Reloaded.WPF` package for the utilities only if desired.

**Reloaded.WPF is not yet currently on NuGet and will not be until a base set of controls are complete, please instead add the repository as a submodule, add existing project to your solution and add a project reference to it.**

## Installing Reloaded.WPF Components

### Theme

Add the theme into the application's main Resource Dictionary as follows.

```xaml
<Application xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:controls="clr-namespace:Reloaded.WPF.Controls;assembly=Reloaded.WPF"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- This dictionary switches locations between design and runtime -->
                <controls:DesignTimeResourceDictionary RunTimeSource="pack://siteoforigin:,,,/Theme/Default/Root.xaml" DesignTimeSource="pack://application:,,,/Reloaded.WPF.Theme.Default;component/Theme/Default/Root.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```


### Windows
![Window Style](https://i.imgur.com/VwE2M95.png)

If you would wish for your window to inherit the base Reloaded style, *including the glowing border* then there are two things you need to do:
- Set window style to `ReloadedWindow` as a `DynamicResource`.
- Inherit window class from `ReloadedWindow`.

#### e.g. Mainwindow.xaml

```xml
<default:ReloadedWindow 
		x:Class="Reloaded.WPF.TestWindow.MainWindow"
		xmlns:default="clr-namespace:Reloaded.WPF.Theme.Default;assembly=Reloaded.WPF.Theme.Default"
		...
        Style="{DynamicResource ReloadedWindow}">
```

#### e.g. Mainwindow.xaml.cs
```csharp
public partial class MainWindow : ReloadedWindow
```

**Warning**

Inheriting from `ReloadedWindow` will replace your window's `DataContext`, which is used by the underlying base window theme to style the window.

For now, Reloaded.WPF expects that you implement all your functionality in pages, (which can be hosted inside a singular `PageHost` in the main window) although this may change in the future.

### Pages

![Page Theme](https://i.imgur.com/ZRfoldh.png)

Inheriting pages is the same as inheriting windows, in this case, you should make the pages inherit `ReloadedPage` instead.

Inheriting the page class provides you page support for animations on entry and exit.

#### e.g. ProcessPage.xaml

```xml
<default:ReloadedPage 
	x:Class="Reloaded.WPF.TestWindow.Pages.ProcessPage"
    xmlns:default="clr-namespace:Reloaded.WPF.Theme.Default;assembly=Reloaded.WPF.Theme.Default">
```

#### e.g. ProcessPage.xaml.cs
```
public partial class ProcessPage : ReloadedPage
```

If you do not wish to inherit the Reloaded theme, then simply instead inherit from `PageBase` *(in Reloaded.WPF.Pages)* and override `MakeEntryAnimations` and `MakeExitAnimations` which will allow you to retain animation support.

See [ReloadedPage](https://github.com/Sewer56/Reloaded.WPF/blob/master/Source/Reloaded.WPF.Theme.Default/ReloadedPage.cs) for an example.

### Using the Designer with .NET Core
This is technically unrelated to Reloaded.WPF but it's very useful and not yet well known.

As you may know, .NET Core 3.X supports WinForms and WPF, but at the time of writing there is no/weak designer support for them in Visual Studio.

Well, it is possible to have a working designer and still build for .NET Core 3 without any time consuming, manual labour involving dirty hacks like adding all files as link to a separate project.

All you need to do is multi-target in your `.csproj`, simply remove the `<TargetFramework>` property and replace it with the `<TargetFrameworks>` property, then target `.NET Framework` as well as `.NET Core`.

```xml
<TargetFrameworks>NET472;netcoreapp3.0</TargetFrameworks>
```

**But make sure .NET Framework comes first!**
