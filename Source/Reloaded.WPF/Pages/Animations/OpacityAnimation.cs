using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Reloaded.WPF.Pages.Animations
{
    /// <summary>
    /// Constructs a <see cref="DoubleAnimation"/> which is used to animate the opacity
    /// of a specified <see cref="FrameworkElement"/>.
    /// </summary>
    public class OpacityAnimation : Animation
    {
        /// <summary> The length of the animation in seconds. </summary>
        public double Duration { get; set; }

        /// <summary> The function that controls the acceleration/deceleration ratio of the slide animation.</summary>
        public IEasingFunction EasingFunction { get; set; }

        /// <summary>
        /// The initial opacity of the element.
        /// </summary>
        public double From { get; set; }

        /// <summary>
        /// The final opacity of the element.
        /// </summary>
        public double To { get; set; }

        /// <param name="duration">The length of the animation in seconds.</param>
        /// <param name="from">The initial opacity of the element.</param>
        /// <param name="to">The final opacity of the element.</param>
        /// <param name="easingFunction">(Optional) The easing function to control the acceleration of the slide animation over a course of time.</param>
        public OpacityAnimation(double duration, double from, double to, IEasingFunction easingFunction = null)
        {
            Duration = duration;
            From = from;
            To = to;

            if (easingFunction == null)
                easingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut };

            EasingFunction = easingFunction;
        }


        public override AnimationTimeline Get()
        {
            var opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(Duration));
            opacityAnimation.From = From;
            opacityAnimation.To = To;
            opacityAnimation.EasingFunction = EasingFunction;

            return opacityAnimation;
        }

        public override void AddToStoryboard(Storyboard storyBoard, AnimationTimeline timeline = null)
        {
            var animation = timeline ?? Get();
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            storyBoard.Children.Add(animation);
        }

        /// <summary>
        /// This method is unused.
        /// </summary>
        public override void PrepareElement(FrameworkElement element)
        {

        }
    }
}
