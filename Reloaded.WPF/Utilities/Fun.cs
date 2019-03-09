using System;
using System.Threading;
using System.Windows.Media;

namespace Reloaded.WPF.Utilities
{
    public class Fun
    {
        /// <summary>
        /// Enables hue cycling of the drop shadow for the window.
        /// </summary>
        /// <param name="setColorFunction">Function that applies a colour to a variable.</param>
        /// <param name="framesPerSecond">The amount of frames per second.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        /// <param name="chroma">Range 0 to 100. The quality of a color's purity, intensity or saturation. </param>
        /// <param name="lightness">Range 0 to 100. The quality (chroma) lightness or darkness.</param>
        /// <remarks>https://www.harding.edu/gclayton/color/topics/001_huevaluechroma.html</remarks>
        public static Thread HueCycleColor(Action<Color> setColorFunction, int framesPerSecond = 30, int duration = 6000, float chroma = 50F, float lightness = 50F)
        {
            Thread hueCycleThread = new Thread(() =>
            {
                SharpFPS fps = new SharpFPS();
                fps.FPSLimit = framesPerSecond;

                double frameTimeMs  = 1000.0 / framesPerSecond;
                int colors          = (int)Math.Round(duration / frameTimeMs);
                var rainbowColors   = ColorInterpolator.GetRainbowColors(chroma, lightness, colors);

                int currentColor    = 0;
                int maxColor        = rainbowColors.Count;

                while (true)
                {
                    fps.StartFrame();
                    setColorFunction(rainbowColors[currentColor].ToColor());

                    currentColor++;
                    currentColor %= maxColor;

                    fps.EndFrame();
                    fps.Sleep();
                }
            });

            hueCycleThread.Start();
            hueCycleThread.IsBackground = true;
            return hueCycleThread;
        }
    }
}
