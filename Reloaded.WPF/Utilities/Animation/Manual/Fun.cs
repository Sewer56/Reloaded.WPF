﻿using System;
using System.Linq;
using System.Threading;
using System.Windows.Media;

namespace Reloaded.WPF.Utilities.Animation.Manual
{
    public class Fun
    {
        /// <summary>
        /// Enables hue cycling of the drop shadow for the window.
        /// </summary>
        /// <param name="setColorFunction">Function that applies a colour to a variable.</param>
        /// <param name="token"></param>
        /// <param name="framesPerSecond">The amount of frames per second.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        /// <param name="chroma">Range 0 to 100. The quality of a color's purity, intensity or saturation. </param>
        /// <param name="lightness">Range 0 to 100. The quality (chroma) lightness or darkness.</param>
        /// <remarks>https://www.harding.edu/gclayton/color/topics/001_huevaluechroma.html</remarks>
        public static async void HueCycleColor(Action<Color> setColorFunction, CancellationToken token, int framesPerSecond = 30, int duration = 6000, float chroma = 50F, float lightness = 50F)
        {
            SharpFPS fps = new SharpFPS();
            fps.FPSLimit = framesPerSecond;

            double frameTimeMs = 1000.0 / framesPerSecond;
            int colors = (int) Math.Round(duration / frameTimeMs);
            var rainbowColors = ColorInterpolator.GetRainbowColors(chroma, lightness, colors);
            var rainbowColorsNative = rainbowColors.Select((x) => x.ToColor()).ToArray();

            int currentColor = 0;
            int maxColor = rainbowColors.Count;

            while (true)
            {
                fps.StartFrame();
                setColorFunction(rainbowColorsNative[currentColor]);

                currentColor++;
                currentColor %= maxColor;

                fps.EndFrame();
                await fps.SleepAsync();

                // Exit if requested.
                if (token.IsCancellationRequested)
                    return;
            }
        }

        /// <summary>
        /// Animates a transition between one colour to another.
        /// </summary>
        /// <param name="setColorFunction">Function that applies a colour to a variable.</param>
        /// <param name="framesPerSecond">The amount of frames per second.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        /// <param name="sourceColor">The colour to animate from. </param>
        /// <param name="targetColor">The colour to animate to.</param>
        /// <remarks>https://www.harding.edu/gclayton/color/topics/001_huevaluechroma.html</remarks>
        public static async void ColorAnimate(Action<Color> setColorFunction, Color sourceColor, Color targetColor, int framesPerSecond = 30, int duration = 6000)
        {
            SharpFPS fps = new SharpFPS();
            fps.FPSLimit = framesPerSecond;

            double frameTimeMs = 1000.0 / framesPerSecond;
            int colors = (int)Math.Round(duration / frameTimeMs);
            var interpolationColors = ColorInterpolator.CalculateIntermediateColours(sourceColor.ToLch(), targetColor.ToLch(), colors);
            var rainbowColorsNative = interpolationColors.Select((x) => x.ToColor()).ToArray();

            for (int x = 0; x < rainbowColorsNative.Length; x++)
            {
                fps.StartFrame();

                setColorFunction(rainbowColorsNative[x]);

                fps.EndFrame();
                await fps.SleepAsync();
            }
        }
    }
}
