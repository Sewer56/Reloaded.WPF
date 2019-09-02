using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Reloaded.WPF.Animations.Enum;
using Reloaded.WPF.Animations.FrameLimiter;

namespace Reloaded.WPF.Animations
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
    public class ManualAnimation<T> : IDisposable
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
        public Action<T> ExecutionMethod { get; set; }

        /// <summary>
        /// (Optional)
        /// Represents an easing function that creates an animation that accelerates and/or decelerates using a given formula.
        /// </summary>
        public IEasingFunction EasingFunction { get; set; }

        /// <summary>
        /// The number of times to repeat the animation. A number of <see cref="ulong.MaxValue"/> signals infinite repeats.
        /// </summary>
        public ulong Repeat { get; set; } = ulong.MaxValue;

        /// <summary>
        /// Defines the active state of the <see cref="ManualAnimation{T}"/>
        /// </summary>
        public ManualAnimationState State { get; internal set; } = ManualAnimationState.NotStarted;

        private Thread _animateThread;

        /// <summary>
        /// Creates an instance of the <see cref="ManualAnimation{T}"/> method helping create custom animations.
        /// </summary>
        /// <param name="duration">The length of the animation in question.</param>
        /// <param name="frameRate">The framerate of the animation in question.</param>
        public ManualAnimation(float duration, float frameRate)
        {
            Duration = duration;
            FrameRate = frameRate;
        }

        /// <summary>
        /// Creates an instance of the <see cref="ManualAnimation{T}"/> method helping create custom animations.
        /// </summary>
        /// <param name="duration">The length of the animation in question.</param>
        /// <param name="frameRate">The framerate of the animation in question.</param>
        /// <param name="easingFunction">Defines the basic functionality of an easing function.</param>
        public ManualAnimation(float duration, float frameRate, IEasingFunction easingFunction)
        {
            Duration = duration;
            FrameRate = frameRate;
            EasingFunction = easingFunction;
        }

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
        public ManualAnimation(float duration, float frameRate, Func<float, T> interpolationMethod, Action<T> executionMethod)
        {
            Duration = duration;
            FrameRate = frameRate;
            InterpolationMethod = interpolationMethod;
            ExecutionMethod = executionMethod;
        }

        /// <summary>
        /// Creates an instance of the <see cref="ManualAnimation{T}"/> method helping create custom animations.
        /// </summary>
        /// <param name="duration">The length of the animation in question.</param>
        /// <param name="frameRate">The framerate of the animation in question.</param>
        /// <param name="interpolationMethod">Defines the interpolation function used that returns a value to pass to the <see cref="ExecutionMethod"/> given a time between 0 and 1. </param>
        /// <param name="easingFunction">Defines the basic functionality of an easing function.</param>
        /// <param name="executionMethod">
        /// Defines the method to execute with the interpolated value returned from <see cref="InterpolationMethod"/>.
        /// The return value of the function decides whether the animation should continue. i.e. if you return false, the animation will terminate.
        /// </param>
        public ManualAnimation(float duration, float frameRate, Func<float, T> interpolationMethod, Action<T> executionMethod, IEasingFunction easingFunction)
        {
            Duration = duration;
            FrameRate = frameRate;
            InterpolationMethod = interpolationMethod;
            ExecutionMethod = executionMethod;
            EasingFunction = easingFunction;
        }

        /// <summary>
        /// Cancels the animation.
        /// </summary>
        public void CancelAsync()
        {
            State = ManualAnimationState.Cancelled;
        }

        /// <summary>
        /// Cancels the animation.
        /// </summary>
        public void Cancel(int maxWaitTimeMs = 100)
        {
            CancelAsync();
            _animateThread?.Join(maxWaitTimeMs);
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public void Pause()
        {
            if (State != ManualAnimationState.Complete || State != ManualAnimationState.Cancelled)
                State = ManualAnimationState.Paused;
        }

        /// <summary>
        /// Resumes the animation.
        /// </summary>
        public void Resume()
        {
            if (State != ManualAnimationState.Complete || State != ManualAnimationState.Cancelled)
                State = ManualAnimationState.Running;
        }

        /// <summary>
        /// Starts an <see cref="ManualAnimation{T}"/> on a background thread.
        /// Cancels an ongoing animation, if exists.
        /// </summary>
        public void Animate()
        {
            if (InterpolationMethod == null || ExecutionMethod == null)
                throw new Exception($"{nameof(InterpolationMethod)} or {nameof(ExecutionMethod)} is null. Both execution and interpolation method must be specified.");

            // Cancel existing animation if necessary.
            Cancel();

            State = ManualAnimationState.Running;
            _animateThread = new Thread(ExecuteManualAnimation);
            _animateThread.Start();

            // Note: Do not use Task.Run or the ThreadPool.
            // They can lead to significant time being taken to start the animations when many animations are started at once.
        }

        private void ExecuteManualAnimation()
        {
            // Setup the frame pacing class.
            SharpFPS fps = new SharpFPS();
            fps.FPSLimit = FrameRate;

            // Add a stopwatch.
            // The stopwatch is a preventative measure to effectively allow for frameskipping.
            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (ulong x = 0; x < Repeat;)
            {
                watch.Restart();

                while (watch.ElapsedMilliseconds < Duration)
                {
                    // Cancel if necessary.
                    while (State == ManualAnimationState.Paused)
                        Thread.Sleep(16);

                    if (State == ManualAnimationState.Cancelled)
                        return;

                    // Do animation and terminate if requested.
                    float normalizedTime = watch.ElapsedMilliseconds / Duration;
                    float elapsedTime;

                    if (EasingFunction != null)
                        elapsedTime = (float)EasingFunction.Ease(normalizedTime);
                    else
                        elapsedTime = normalizedTime;

                    T nextValue             = InterpolationMethod(elapsedTime);
                    ExecutionMethod(nextValue);

                    // Sleep
                    fps.EndFrame();
                }

                // Increment and reset if going to hit max value.
                x++;

                if (x == ulong.MaxValue)
                    x = 0;
            }

            State = ManualAnimationState.Complete;
        }

        ~ManualAnimation()
        {
            Dispose();
        }

        public void Dispose()
        {
            Cancel();
            GC.SuppressFinalize(this);
        }
    }
}
