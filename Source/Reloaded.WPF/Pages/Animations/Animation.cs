using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Reloaded.WPF.Pages.Animations
{
    /// <summary>
    /// Common interface shared by all animations which defines
    /// a singular WPF animation to apply to controls.
    /// </summary>
    public abstract class Animation
    {
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
        public virtual void PrepareElement(FrameworkElement element) { }

        /// <summary>
        /// Shorthand for calling <see cref="Animation.PrepareElement"/> and <see cref="Animation.AddToStoryboard"/>.
        /// </summary>
        public static void AddAnimation(Storyboard storyBoard, Reloaded.WPF.Pages.Animations.Animation animation, FrameworkElement element = null)
        {
            animation.PrepareElement(element);
            animation.AddToStoryboard(storyBoard);
        }

        /// <summary>
        /// Adds an array of <see cref="Animation"/>s to the storyboard.
        /// </summary>
        public static void AddAnimations(Storyboard storyBoard, IEnumerable<Reloaded.WPF.Pages.Animations.Animation> animations, FrameworkElement element = null)
        {
            foreach (var animation in animations)
            {
                AddAnimation(storyBoard, animation, element);
            }   
        }

        /// <summary>
        /// Animates an individual framework element.
        /// </summary>
        /// <param name="animations">The animations to perform on the framework element.</param>
        /// <param name="element">The framework element to animate.</param>
        public static void Animate(Animation[] animations, FrameworkElement element)
        {
            var storyBoard = new Storyboard();
            Animation.AddAnimations(storyBoard, animations, element);
            storyBoard.Begin(element);
        }

        /// <summary>
        /// Retrieves the longest animation assigned to a storyboard in seconds.
        /// </summary>
        public static double GetLongestAnimationDuration(Storyboard storyBoard)
        {
            return storyBoard.Children.Max(x => x.Duration.TimeSpan.TotalSeconds);
        }
    }
}
