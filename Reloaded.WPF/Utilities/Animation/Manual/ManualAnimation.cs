using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Reloaded.WPF.Utilities.Animation.Manual
{
    /// <summary>
    /// <see cref="ManualAnimation{T}"/> is small utility wrapper class allowing you to make animations manually by providing an interpolation and execution function.
    /// Note: This class is best explained by example, if confused please see the wiki/markdown for Reloaded.WPF for more details.
    /// </summary>
    /// <remarks>
    /// Once an animation begins, changes made to the class properties do not take effect until the animation is played again.
    /// Regarding timing of the <see cref="ManualAnimation{T}"/>, the class implements frameskipping. This means that <see cref="ExecutionMethod"/>
    /// consistently takes too long to execute, the overall length of the animation will not be extended.
    /// </remarks>
    /// <typeparam name="T">The type of the property that is to be animated.</typeparam>
    public class ManualAnimation<T>
    {
        /// <summary>
        /// The length of the animation, in milliseconds.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// How many times per second should the animation play at.
        /// </summary>
        public float FrameRate { get; set; }

        /// <summary>
        /// Defines the interpolation function used that returns a value to pass to the <see cref="ExecutionMethod"/> given a time between 0 and 1.
        /// Tip: The interpolation function should probably be linear if using a custom <see cref="EasingFunction"/>. 
        /// </summary>
        public Func<float, T> InterpolationMethod { get; set; }

        /// <summary>
        /// Defines the method to execute with the interpolated value returned from <see cref="InterpolationMethod"/>.
        /// </summary>
        public Func<T, bool> ExecutionMethod { get; set; }

        /// <summary>
        /// (Optional)
        /// Represents an easing function that creates an animation that accelerates and/or decelerates using a given formula.
        /// </summary>
        public IEasingFunction EasingFunction { get; set; }

        /// <summary>
        /// The number of times to repeat the animation. A number of <see cref="ulong.MaxValue"/> signals infinite repeats.
        /// </summary>
        public ulong Repeat { get; set; } = 1;

        // progress = EasingFunction.Ease(time); // Range of time and progress. 0-1.

        /// <summary>
        /// Creates an instance of the <see cref="ManualAnimation{T}"/> method helping create custom animations.
        /// </summary>
        /// <param name="duration">The length of the animation in question.</param>
        /// <param name="frameRate">The framerate of the animation in question.</param>
        /// <param name="interpolationMethod">Defines the interpolation function used that returns a value to pass to the <see cref="ExecutionMethod"/> given a time between 0 and 1. </param>
        /// <param name="executionMethod">
        /// Defines the method to execute with the interpolated value returned from <see cref="InterpolationMethod"/>.
        /// The return value of the function decides whether the animation should continue. i.e. if you return false, the animation will terminate.
        /// </param>
        public ManualAnimation(float duration, float frameRate, Func<float, T> interpolationMethod, Func<T, bool> executionMethod)
        {
            Duration = duration;
            FrameRate = frameRate;
            InterpolationMethod = interpolationMethod;
            ExecutionMethod = executionMethod;
        }

        /// <summary>
        /// Plays the <see cref="ManualAnimation{T}"/>.
        /// </summary>
        public async Task AnimateAsync()
        {
            if (InterpolationMethod == null || ExecutionMethod == null)
                throw new Exception($"{nameof(InterpolationMethod)} or {nameof(ExecutionMethod)} is null. Both execution and interpolation method must be specified.");

            // Local copy animation parameters.
            // This is a safeguard against changing properties mid animation.
            float framesPerSecond = FrameRate;
            float duration = Duration;
            var executionMethod = ExecutionMethod;
            var interpolationMethod = InterpolationMethod;
            var easingFunction = EasingFunction;
            ulong repeat = Repeat;

            // Setup the frame pacing class.
            SharpFPS fps = new SharpFPS();
            fps.FPSLimit = framesPerSecond;

            // Add a stopwatch.
            // The stopwatch is a preventative measure to effectively allow for frameskipping.
            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (ulong x = 0; x < repeat;)
            {
                watch.Restart();

                while (watch.ElapsedMilliseconds < duration)
                {
                    // Start frame.
                    fps.StartFrame();

                    // Do animation and terminate if requested.
                    float normalizedTime = watch.ElapsedMilliseconds / duration;
                    float elapsedTime;

                    if (easingFunction != null)
                        elapsedTime = (float) easingFunction.Ease(normalizedTime);
                    else
                        elapsedTime = normalizedTime;

                    T nextValue = interpolationMethod(elapsedTime);
                    bool continueAnimation = executionMethod(nextValue);

                    if (!continueAnimation)
                        return;

                    // Sleep
                    fps.EndFrame();
                    await fps.SleepAsync();
                }

                // Increment and reset if going to hit max value.
                x++;

                if (x == ulong.MaxValue)
                    x = 0;
            }
        }
    }
}
