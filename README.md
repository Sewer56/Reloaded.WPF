

<div align="center">
	<h1>Project Reloaded: WPF GUI</h1>
	<img src="https://i.imgur.com/BjPn7rU.png" width="150" align="center" />
	<br/> <br/>
	<strong><i>The future of Reloaded GUI utilities</i></strong>
	<br/> <br/>
	<!-- Build Status -->
	<a href="https://ci.appveyor.com/project/sewer56lol/reloaded-wpf">
		<img src="https://ci.appveyor.com/api/projects/status/26icfl39yoa0alvi?svg=true" alt="Build Status" />
	</a>
</div>

# Reloaded.WPF

Reloaded.WPF is a miniature WPF library/toolkit made for personal use, featuring a reusable set of bare essentials for building simple Reloaded branded WPF applications.

The toolkit consists of basic controls, animations, base classes for implementing e.g. AttachedProperties and utilities as well as a custom from the ground up theme resembling that of the original [Reloaded Mod Loader](https://github.com/Sewer56/Reloaded-Mod-Loader).

Features/controls/styles to this project are added as "Reloaded", both the mod loader *Reloaded II* and various other utilities require them.

*Reloaded.WPF supports .NET Core 3.X+ as well as .NET Framework.*

## Style/Theme Preview

### Visual Window State
![](https://i.imgur.com/Pvo8BPt.gif)

By default, when the window is inactive, the border automatically turns gray. Once it becomes active, the border automatically animates to a color depending on the window "state".

There are currently 2 window states implemented. You have "Normal" *(white)* when nothing special is going on and "Engaged" *(maroon)* for when the program is busy/doing some work.

### Animations
 
![Animation](https://i.imgur.com/NBbniBo.gif)

The Reloaded theme supports a minimal set of default animations in its own very easy to use animation interface based on storyboards, with animations being very easy to make and attach to UI elements. 

In the above case we see the default page entry/exit animations running on the Reloaded animation interface, hosted inside Reloaded's `PageHost` control. Animating in and out is asynchronous, meaning that both the old page can animate out and the new page can animate in at the same time.

Need to animate something manually unrelated to WPF UI elements? Got tools for that too!

## Core Properties

### Minimal

Many toolkits, frameworks and libraries are large and may require a lot of time to study. They range from huge, complete sets of controls and themes to interesting, opinionated alterations on how you interact with WPF at the core level itself.

Reloaded.WPF is neither, this is only a simple set of bare essentials to make development of WPF apps quicker. It can easily be combined with other frameworks to your own desire.

The barrier to entry remains at the baseline level of WPF knowledge.

### Lightweight

For some projects, bloat is a rather major concern. As such, Reloaded.WPF consists of only two minimal assemblies, base and the default theme. In both of the assemblies, there are no external dependencies outside the .NET Base Class Library (BCL).

The footprint of Reloaded.WPF is approximately 450KB in Release mode, of which 70KB is code and the remaining 380KB are assets (Resource Dictionaries, Images, Fonts).

### Customizable

As with all of the `Reloaded-Project` projects and my free and open source repositories, the primary idea is to empower the end user as much as possible.

The Reloaded WPF theme is loaded dynamically at runtime from disk therefore entirely customizable by both the developer and the end user.

When you compile an application using the Reloaded WPF theme, you will notice that all of the theme's styles are on the filesystem and can be edited directly in any plain old text editor.

![Filesystem](https://i.imgur.com/DbBQj2u.png)

![Settings](https://i.imgur.com/xSMvkTu.png)

Want to make changes to the defaults or changes specific to your application?
Fork this repo! Forks are encouraged!

#### Example
![Rethemed Window](https://i.imgur.com/RzFkE8v.png)

*Edited `AccentColor` and `AccentColorLight`in `Colors.xaml`, shifting their hue by 75 in the LCh color space.* 

## Resources
- [Getting Started](Docs/Getting-Started.md)


## Contributions
As with the standard for all of the  `Reloaded-Project`, repositories; contributions are very welcome and encouraged.

Feel free to implement new features, make bug fixes or suggestions so long as they are accompanied by an issue with a clear description of the pull request.

## Licenses, Authors & Contributions

Reloaded.WPF uses a heavily stripped down version of the `ColorMine` library (git repository no longer available) written by THEjoezack under the [MIT License](https://opensource.org/licenses/MIT).

Reloaded.WPF uses a simple implementation of the Circular Buffer by Joao Portela, licensed under the "beer-ware" license and available here: [Circular Buffer](https://github.com/joaoportela/CircullarBuffer-CSharp). *(Notice retained in Circular Buffer source code).*

Reloaded.WPF uses the `WindowResizer` class for improving compatibility of custom borderless windows and receiving dock events written by Luke Malpass (AngelSix) as part of [fasetto-word](https://github.com/angelsix/fasetto-word](https://github.com/angelsix/fasetto-word) licensed under the [MIT License](https://opensource.org/licenses/MIT).

Inspiration for the `BindingProxy` class was taken from a blog post by Thomas Levesque: https://thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/ .

*All other code, unless specified is licensed using the GNU GPL v3 license.*
