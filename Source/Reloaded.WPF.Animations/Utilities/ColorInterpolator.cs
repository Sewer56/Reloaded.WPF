using Reloaded.WPF.ColorMineLite.ColorSpaces;

namespace Reloaded.WPF.Animations.Utilities
{
    /// <summary>
    /// Utility class that provides various colour interpolations between sets of colours.
    /// </summary>
    public static class ColorInterpolator
    {
        /// <summary>
        /// Calculates an intermediate colour between Colour X and Colour Y.
        /// </summary>
        /// <param name="sourceColor">Colour interpolation begins from.</param>
        /// <param name="destinationColor">Colour interpolation ends up.</param>
        /// <param name="time">A normalized time between 0-1 which dictates the interpolated colour. The formula for the returned colour is "sourceColor + ((destinationColor - sourceColor) * time)" for each of the 3 LCh components.</param>
        public static Lch CalculateIntermediateColour(Lch sourceColor, Lch destinationColor, float time)
        {
            // Calculate the differences of LCH from source to destination.
            double hDelta = destinationColor.H - sourceColor.H;
            double cDelta = destinationColor.C - sourceColor.C;
            double lDelta = destinationColor.L - sourceColor.L;

            return new Lch
            {
                L = sourceColor.L + lDelta * time,
                C = sourceColor.C + cDelta * time,
                H = sourceColor.H + hDelta * time
            };
        }

        /// <summary>
        /// Retrieves a colour of the rainbow with a specified chroma and lightness.
        /// </summary>
        /// <param name="chroma">Range 0 to 100. The quality of a color's purity, intensity or saturation. </param>
        /// <param name="lightness">Range 0 to 100. The quality (chroma) lightness or darkness.</param>
        /// <param name="time">A normalized time between 0-1 which dictates the hue of the colour. The hue ranges between 0 to 360 and is calculated by time * 360.</param>
        public static Lch GetRainbowColor(float chroma, float lightness, float time)
        {
            float hue = time * 360.0F;
            return new Lch() { L = lightness, C = chroma, H = hue };
        }
    }
}
