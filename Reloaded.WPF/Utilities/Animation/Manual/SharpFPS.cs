using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Reloaded.WPF.Utilities.Animation.Manual
{
    /// <summary>
    /// The SharpFPS class is a simple class that automatically calculates the current rendered amount of frames per second
    /// using Windows' high resolution event timer.
    /// </summary>
    public class SharpFPS
    {
        private const float MillisecondsInSecond = 1000;

        /// <summary>
        /// Contains the stopwatch used for timing.
        /// </summary>
        public Stopwatch LocalStopwatch;

        // ----------------------------------------------------
        // User configurable

        /// <summary>
        /// Sets or gets the current framerate cap.
        /// </summary>
        public float FPSLimit
        {
            get { return _FPSLimit; }
            set
            {
                FrameTime = MillisecondsInSecond / value;
                _FPSLimit = value;
            }
        }

        private float _FPSLimit;

        /// <summary>
        /// [Milliseconds] Contains the current set maximum allowed time that a frame should be rendered in.
        /// This value is automatically generated when you set the <see cref="FPSLimit"/>.
        /// </summary>
        public double FrameTime { get; private set; }

        // ----------------------------------------------------

        /// <summary>
        /// Contains the current amount of frames per second.
        /// </summary>
        public double FPS { get; private set; }

        /// <summary>
        /// Contains the current amount of frames per second in the case the FPS limit were to be removed.
        /// </summary>
        public double PotentialFPS { get; private set; }

        /// <summary>
        /// [Milliseconds] The amount spent rendering the last frame.
        /// </summary>
        public double RenderTime { get; private set; }

        /// <summary>
        /// [Milliseconds] The time that will be spent sleeping should <see cref="Sleep"/> be called until the next frame will be rendered.
        /// </summary>
        public double SleepTime { get; private set; }

        /// <summary>
        /// The SharpFPS class is a simple class that automatically calculates the current rendered amount of frames per second
        /// using Windows' high resolution event timer.
        /// To use this class, call "StartFrame" at the start of your render loop, and EndFrame at the end of your render loop.
        /// </summary>
        public SharpFPS()
        {
            LocalStopwatch = new Stopwatch();
            FPSLimit = 144;
        }

        /// <summary>
        /// Updates the current internal FPS counter of the <see cref="SharpFPS"/> class.
        /// You should call this after every frame.
        /// </summary>
        /// <returns>The current estimated amount of frames per second.</returns>
        public void StartFrame()
        {
            // Restart the stopwatch.
            LocalStopwatch.Restart();
        }

        /// <summary>
        /// Updates the current internal FPS counter of the <see cref="SharpFPS"/> class.
        /// You should call this after every frame and right before sleep.
        /// </summary>
        public void EndFrame()
        {
            // The total amount of milliseconds taken for rendering.
            double millisecondsElapsed = TicksToMilliseconds(LocalStopwatch.ElapsedTicks);

            // Calculate the various times.
            RenderTime = millisecondsElapsed;
            SleepTime = FrameTime - RenderTime;
            PotentialFPS = MillisecondsInSecond / millisecondsElapsed;

            // Calculate FPS if dipped below target.
            if (SleepTime < 0)
            {
                SleepTime = 0;
                FPS = MillisecondsInSecond / millisecondsElapsed;
            }
            else
            {
                FPS = FPSLimit;
            }
        }

        /// <summary>
        /// Pauses execution for the remaining of the time until the next frame begins.
        /// </summary>
        /// <param name="spin">
        ///     If true, briefly spins after sleeping (blocks using a while loop) until it is precisely the time to start the next frame.
        ///     Increases accuracy at the expense of CPU load.
        /// </param>
        public void Sleep(bool spin = true)
        {
            int sleepMilliseconds = (int)Math.Floor(SleepTime);

            // Sleep down to the nearest millisecond.
            if (sleepMilliseconds >= 1)
                Thread.Sleep(sleepMilliseconds);

            // Spin for the remaining estimate of ticks.
            if (spin)
                Spin();
        }

        /// <summary>
        /// Pauses execution for the remaining of the time until the next frame begins.
        /// </summary>
        /// <param name="spin">
        ///     If true, briefly spins after sleeping (blocks using a while loop) until it is precisely the time to start the next frame.
        ///     Increases accuracy at the expense of CPU load.
        /// </param>
        public async Task SleepAsync(bool spin = true)
        {
            int sleepMilliseconds = (int)Math.Floor(SleepTime);

            // Sleep down to the nearest millisecond.
            if (sleepMilliseconds >= 1)
                await Task.Delay(sleepMilliseconds);

            // Spin for the remaining estimate of ticks.
            if (spin)
                Spin();
        }


        private void Spin()
        {
            long frameTicks = MillisecondsToTicks(FrameTime);
            while (LocalStopwatch.ElapsedTicks < frameTicks)
            { }
        }

        /// <summary>
        /// Converts the ticks of a <see cref="Stopwatch"/> into milliseconds.
        /// </summary>
        /// <returns></returns>
        private long MillisecondsToTicks(double milliseconds)
        {
            long ticksPerMillisecond = Stopwatch.Frequency / 1000;
            return (long)(ticksPerMillisecond * milliseconds);
        }

        /// <summary>
        /// Converts the ticks of a <see cref="Stopwatch"/> into milliseconds.
        /// </summary>
        /// <returns></returns>
        private double TicksToMilliseconds(long ticks)
        {
            long ticksPerMillisecond = Stopwatch.Frequency / 1000;
            return (double)(ticks / (double)ticksPerMillisecond);
        }
    }
}