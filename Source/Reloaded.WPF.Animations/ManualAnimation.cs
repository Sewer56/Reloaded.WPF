﻿using System;
using System.Diagnostics;
using System.Threading;
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
        /// Executed when the animation is cancelled.
        /// </summary>
        public event StateChanged OnStateChange; 

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
        /// The current amount of times the animation has been repeated.
        /// Starts with 0.
        /// </summary>
        public ulong RepeatCount { get; private set; } = 0;

        /// <summary>
        /// The time this animation has spent running in total.
        /// </summary>
        public float TimeRunning { get; private set; } = 0.0f;

        /// <summary>
        /// Defines the active state of the <see cref="ManualAnimation{T}"/>
        /// </summary>
        public ManualAnimationState State
        {
            get => _state;
            internal set
            {
                var changed = _state != value;
                var before  = _state;
                _state = value;
                
                if (changed)
                    OnStateChange?.Invoke(before, _state);
            }
        }

        private Thread _animateThread;
        private ManualAnimationState _state = ManualAnimationState.NotStarted;

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
        public void Cancel(int maxWaitTimeMs = 200)
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
            AnimateManual();
            AnimateAutomatic();
        }

        /// <summary>
        /// Resets the <see cref="ManualAnimation{T}"/> for 'manual' use.
        /// Manual use requires that you call <see cref="ManualUpdate"/> every time the animation state is to be updated.
        /// </summary>
        public void AnimateManual()
        {
            if (InterpolationMethod == null || ExecutionMethod == null)
                throw new Exception($"{nameof(InterpolationMethod)} or {nameof(ExecutionMethod)} is null. Both execution and interpolation method must be specified.");

            // Cancel existing animation if necessary.
            Cancel();

            State = ManualAnimationState.Running;
        }

        private void AnimateAutomatic()
        {
            _animateThread = new Thread(ExecuteAutomaticAnimation);
            _animateThread.IsBackground = true;
            _animateThread.Start();

            // Note: Do not use Task.Run or the ThreadPool.
            // They can lead to significant time being taken to start the animations when many animations are started at once.
        }

        private void ExecuteAutomaticAnimation()
        {
            // Setup the frame pacing class.
            SharpFPS fps = new SharpFPS();
            fps.FPSLimit = FrameRate;

            // Add a stopwatch.
            // The stopwatch is a preventative measure to effectively allow for frameskipping.
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var lastWatchTime = watch.Elapsed.TotalMilliseconds;

            while (true)
            {
                // Update Delta Time.
                var currentWatchTime = watch.Elapsed.TotalMilliseconds;
                var deltaTime = currentWatchTime - lastWatchTime;
                lastWatchTime = currentWatchTime;

                ManualUpdate((float)deltaTime);

                // Check if to terminate thread.
                if (State == ManualAnimationState.Cancelled || State == ManualAnimationState.Complete)
                    return;

                fps.EndFrame();
            }
        }

        /// <summary>
        /// Can be used to manually update the animation without the use of the background thread.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last call.</param>
        public void ManualUpdate(float deltaTime)
        {
            // Check if we're done.
            if (RepeatCount >= Repeat)
            {
                State = ManualAnimationState.Complete;
                return;
            }

            // Otherwise return if paused.
            if (State == ManualAnimationState.Paused || State == ManualAnimationState.Cancelled)
                return;

            // Or execute the logic.
            TimeRunning += deltaTime;
            Update();

            // Advance to next repeat if necessary.
            if (TimeRunning > Duration)
            {
                TimeRunning %= Duration;
                RepeatCount++;
            }
        }

        private void Update()
        {
            float normalizedTime = TimeRunning / Duration;
            if (normalizedTime > 1.0f)
                normalizedTime = 1.0f;

            float elapsedTime;

            if (EasingFunction != null)
                elapsedTime = (float)EasingFunction.Ease(normalizedTime);
            else
                elapsedTime = normalizedTime;

            T nextValue = InterpolationMethod(elapsedTime);
            ExecutionMethod(nextValue);
        }

        /// <inheritdoc />
        ~ManualAnimation()
        {
            Dispose();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Cancel();
            GC.SuppressFinalize(this);
        }

        public delegate void StateChanged(ManualAnimationState before, ManualAnimationState after);
    }
}
