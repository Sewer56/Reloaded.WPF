<div align="center">
	<h1>Reloaded.WPF: Animations</h1>
	<img src="https://i.imgur.com/BjPn7rU.png" width="150" align="center" />
	<br/> <br/>
	<strong><i>You expected cringeworthy text, but <strike>nobody</strike> nothing came!</i></strong>
</div>

## Page Information

🕒  Reading Time: 5 Minutes

## Prologue

Within Reloaded.WPF there exist two types of animation.

First, there is `Animation` (an `abstract` class), which are executed by `Storyboard`(s) and are intended to animate controls, living within the confines of WPF. 

Second, should you ever need to escape the confines of WPF, there is an animation type called `ManualAnimation`, which helps you to create animations manually provided you provide an interpolation and execution function. 

They can in theory be used interchangeably with each other, though are separated for the purposes of convenience, the former uses existing APIs and is more appropriate to use with WPF elements while the latter is minimal and completely custom.

## WPF Animations (Animation)

The base class, as well as pre-existing animations can be found in the `Reloaded.WPF.Pages.Animations` namespace.

To create your own animation, all that is simply required is to inherit the `Animation` class and override 
the `Get`, `AddToStoryboard` and (Optional) `PrepareElement` function(s).

```csharp
/// <summary>
/// Constructs the animation and returns the individual instance of the animation.
/// </summary>
/// <returns>An instance of an <see cref="AnimationTimeline"/> (see animation documentation for true type) that may be modified by the user.</returns>
public abstract AnimationTimeline Get();

/// <summary>
/// Constructs the animation and adds it to an existing <see cref="Storyboard"/>.
/// </summary>
/// <param name="storyBoard">The storyboard to add the animation to.</param>
/// <param name="timeline">(Optional) Modified version of the timeline obtained from <see cref="Get"/>. </param>
public abstract void AddToStoryboard(Storyboard storyBoard, AnimationTimeline timeline = null);

/// <summary>
/// (Optional)
/// Makes modifications (if necessary) to a WPF element to allow for the animation to function.
/// This should be specified in XML documentation as not necessary if not required.
/// Example usages: Add transform to element if not exists.
/// </summary>
public abstract void PrepareElement(FrameworkElement element);
```

In the case of this type of animation, an example is worth more than a 1000 words, therefore I would recommend looking at the default implementation of [OpacityAnimation](https://github.com/Sewer56/Reloaded.WPF/blob/63e58e631a0df06a73cd3b2c960de03e9ea4c0e3/Source/Reloaded.WPF/Pages/Animations/OpacityAnimation.cs).

## Custom Animations (ManualAnimation)

In the case of non-WPF animations, the `ManualAnimation` class in the `Reloaded.WPF.Utilities.Animation.Manual` namespace exists as a helper function to help you create animations.

Specifically, `ManualAnimation` takes care of all of the miscellaneous things for you such as timing (duration), frame pacing, easing, cancelling and repeating of the animation.

This simply leaves for you to provide the function that provides an interpolation (given a number 0.0 to 1.0, returns a value to set) and a function that provides the execution (set, do something with that value!).

**Example Interpolation Function**
```csharp
/// <summary>
/// Calculates an intermediate colour between Colour X and Colour Y.
/// </summary>
/// <param name="sourceColor">Colour interpolation begins from.</param>
/// <param name="destinationColor">Colour interpolation ends up.</param>
/// <param name="time">A normalized time between 0-1 which dictates the interpolated colour. The formula for the returned colour is "sourceColor + ((destinationColor - sourceColor) * time)" for each of the 3 LCh components.</param>
public static Lch CalculateIntermediateColour(Lch sourceColor, Lch destinationColor, float time)
{
    // Calculate the differences of LCH from source to destination.
    double hDelta = destinationColor.H - sourceColor.H;
    double cDelta = destinationColor.C - sourceColor.C;
    double lDelta = destinationColor.L - sourceColor.L;

    return new Lch
    {
        L = sourceColor.L + lDelta * time,
        C = sourceColor.C + cDelta * time,
        H = sourceColor.H + hDelta * time
    };
}
```

**Example Execution Function**
```csharp
bool executionMethod(Color color)
{
	// Set the color of some property/element etc.
	element.Color = color;
    return true;
}
```

**Example Usage**
```csharp
var animation = new ManualAnimation<Color>(duration, framesPerSecond,
										   time => ColorInterpolator.CalculateIntermediateColour(lchSourceColor, lchTargetColor, time).ToColor(),
										   executionMethod);
animation.Animate();
```

Some example animations can be found in the `Reloaded.WPF.Utilities.Animation.Manual` namespace, under the class `ManualAnimations`.
