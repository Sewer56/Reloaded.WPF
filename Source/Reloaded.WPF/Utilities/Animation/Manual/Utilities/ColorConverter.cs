using System;
using System.Collections.Generic;
using System.Windows.Media;
using ColorMine.ColorSpaces;

namespace Reloaded.WPF.Utilities.Animation.Manual.Utilities
{
    /// <summary>
    /// Colour c, suitable for HUE cycling and interpolation
    /// </summary>
    public static class ColorConverter
    {
        /// <summary>
        /// Converts System.Windows.Media.Color to LCH colourspace.
        /// </summary>
        public static Lch ToLch(this Color color)
        {
            Rgb rgbColor = new Rgb {R = color.R, G = color.G, B = color.B};
            return rgbColor.To<Lch>();
        }

        /// <summary>
        /// Converts LCH colour back to System.Windows.Media.Color.
        /// </summary>
        public static Color ToColor(this Lch lchColor)
        {
            Rgb rgbColor = lchColor.To<Rgb>();

            byte R = (byte) Math.Round(rgbColor.R, MidpointRounding.AwayFromZero);
            byte G = (byte) Math.Round(rgbColor.G, MidpointRounding.AwayFromZero);
            byte B = (byte) Math.Round(rgbColor.B, MidpointRounding.AwayFromZero);
            
            return Color.FromArgb(255, R, G, B);
        }

        /// <summary>
        /// Converts a collection of LCH colours to Colo.
        /// </summary>
        public static List<Color> ToColor(IEnumerable<Lch> lchColors)
        {
            List<Color> colorList = new List<Color>();
            
            foreach (var colorLch in lchColors)
                colorList.Add(ToColor(colorLch));

            return colorList;
        }

        /// <summary>
        /// Converts a list of colours to LCH.
        /// </summary>
        public static List<Lch> ToLch(IEnumerable<Color> colorList)
        {
            List<Lch> lchList = new List<Lch>();
            foreach (Color color in colorList)
                lchList.Add(ToLch(color));


            return lchList;
        }
    }
}
