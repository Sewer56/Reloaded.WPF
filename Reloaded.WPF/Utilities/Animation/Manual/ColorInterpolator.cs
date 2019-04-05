using System.Collections.Generic;
using Colourful;

namespace Reloaded.WPF.Utilities.Animation.Manual
{
    public static class ColorInterpolator
    {
        /// <summary>
        /// Calculates all of the intermediate colours between colour X and colour Y.
        /// </summary>
        /// <param name="sourceColor">Colour interpolation begins from.</param>
        /// <param name="destinationColor">Colour interpolation ends up.</param>
        /// <param name="iterations">Colours to calculate between X and Y.</param>
        /// <remarks>Final list of colours includes both source and destination colours at first and last element.</remarks>
        public static List<LChabColor> CalculateIntermediateColours(LChabColor sourceColor, LChabColor destinationColor, int iterations)
        {
            // Calculate the differences of LCH from source to destination.
            double hDelta = destinationColor.h - sourceColor.h;
            double cDelta = destinationColor.C - sourceColor.C;
            double lDelta = destinationColor.L - sourceColor.L;

            // Store list of colours.
            List<LChabColor> colours = new List<LChabColor>(iterations);
            colours.Add(sourceColor);

            // Calculate all intermediate colours.
            for (int x = 1; x < iterations; x++)
            {
                // Calculate percentage progress and scale delta between source and destination.
                double percentageProgress = (float)x / iterations;

                double hScaled = hDelta * percentageProgress;
                double cScaled = cDelta * percentageProgress;
                double lScaled = lDelta * percentageProgress;

                // Add a new colour which is a combination of the source value with added scaled delta
                colours.Add
                (
                    new LChabColor
                    (
                        sourceColor.L + lScaled,
                        sourceColor.C + cScaled,
                        sourceColor.h + hScaled
                    )
                );
            }

            colours.Add(destinationColor);
            return colours;
        }

        /// <summary>
        /// Gets all of the colours of the rainbow with a specified chroma and lightness.
        /// </summary>
        /// <param name="chroma">Range 0 to 100. The quality of a color's purity, intensity or saturation. </param>
        /// <param name="lightness">Range 0 to 100. The quality (chroma) lightness or darkness.</param>
        /// <remarks>https://www.harding.edu/gclayton/color/topics/001_huevaluechroma.html</remarks>
        public static List<LChabColor> GetRainbowColors(float chroma = 50F, float lightness = 50F, int colors = 360)
        {
            var lchColorList = new List<LChabColor>(colors);
            double hStepDelta = 360.0 / colors;

            // H = 360 and H = 0 will be the same.
            for (double i = 0; i < colors; i += 1)
                lchColorList.Add(new LChabColor(lightness, chroma, i * hStepDelta));

            return lchColorList;
        }

    }
}
