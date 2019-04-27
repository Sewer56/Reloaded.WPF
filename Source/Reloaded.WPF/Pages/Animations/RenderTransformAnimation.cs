using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Reloaded.WPF.Pages.Animations.Enum;

namespace Reloaded.WPF.Pages.Animations
{
    /// <summary>
    /// Constructs a <see cref="DoubleAnimation"/>, is applied to rendertransform ("RenderTransform") of a <see cref="FrameworkElement"/> allowing
    /// it to slide from a specified offset to the current/default location.
    /// </summary>
    public class RenderTransformAnimation : Animation
    {
        private const string RENDERTRANSFORM_OBJECT_NAME = "RenderTransformAnimation";

        /// <summary> The length of the animation in seconds. </summary>
        public double Duration { get; set; }

        /// <summary> The amount of pixels the object is to travel in the specified X/Y direction. </summary>
        public double Offset { get; set; }

        /// <summary> The function that controls the acceleration/deceleration ratio of the slide animation.</summary>
        public IEasingFunction EasingFunction { get; set; }

        /// <summary> Defines the transform as either horizontal or vertical. </summary>
        public RenderTransformDirection Direction { get; set; }

        /// <summary> Specifies if the <see cref="RenderTransformAnimation"/> is to move away or towards the original position. </summary>
        public RenderTransformTarget Target { get; set; }

        /// <param name="offset">The amount of pixels the object is to travel in the specified X/Y direction. </param>
        /// <param name="direction">Defines the transform as either horizontal or vertical. </param>
        /// <param name="target">Specifies if the <see cref="RenderTransformAnimation"/> is to move away or towards the original position.</param>
        /// <param name="easingFunction">(Optional) The easing function to control the acceleration of the slide animation over a course of time. </param>
        /// <param name="duration">(Optional) The length of the animation in seconds.</param>
        public RenderTransformAnimation(double offset, RenderTransformDirection direction, RenderTransformTarget target, IEasingFunction easingFunction = null, double duration = 0.666F)
        {
            Offset = offset;
            Target = target;
            Direction = direction;
            Duration = duration;

            if (easingFunction == null)
                easingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut };

            EasingFunction = easingFunction;
        }


        /// <inheritdoc/>
        public override AnimationTimeline Get()
        {
            var slideAnimation = new DoubleAnimation();
            slideAnimation.Duration = new Duration(TimeSpan.FromSeconds(Duration));

            if (Target == RenderTransformTarget.Away)
            {
                slideAnimation.From = 0;
                slideAnimation.To = Offset;
            }
            else
            {
                slideAnimation.From = Offset;
                slideAnimation.To = 0;
            }

            slideAnimation.EasingFunction = EasingFunction;

            return slideAnimation;
        }


        /// <inheritdoc/>
        public override void AddToStoryboard(Storyboard storyBoard, AnimationTimeline timeline = null)
        {
            var animation = timeline ?? Get();
            Storyboard.SetTargetName(animation, RENDERTRANSFORM_OBJECT_NAME);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Direction == RenderTransformDirection.Horizontal ? "X" : "Y"));

            storyBoard.Children.Add(animation);
        }

        /// <inheritdoc/>
        public override void PrepareElement(FrameworkElement element)
        {
            // If it does not, add one to existing group or make one if no transform exists.
            if (element.FindName(RENDERTRANSFORM_OBJECT_NAME) == null)
            {
                var translateTransform = new TranslateTransform();
                element.RegisterName(RENDERTRANSFORM_OBJECT_NAME, translateTransform);

                if (element.RenderTransform == null)
                    element.RenderTransform = translateTransform;

                else if (element.RenderTransform == Transform.Identity)
                    element.RenderTransform = translateTransform;

                else if (element.RenderTransform is TransformGroup transformGroup)
                    transformGroup.Children.Add(translateTransform);
            }
        }
    }
}
