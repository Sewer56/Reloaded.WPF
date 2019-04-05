using System;
using System.Collections.Generic;
using System.Windows.Media;
using Colourful;
using Colourful.Conversion;

namespace Reloaded.WPF.Utilities.Animation.Manual
{
    /// <summary>
    /// Colour c, suitable for HUE cycling and interpolation
    /// </summary>
    public static class ColorConverter
    {
        private static ColourfulConverter _converter = new ColourfulConverter();

        /// <summary>
        /// Converts System.Windows.Media.Color to LCH colourspace.
        /// </summary>
        public static LChabColor ToLch(this Color color)
        {
            RGBColor rgbColor = new RGBColor(color.R / 255.0, color.G / 255.0, color.B / 255.0);
            return _converter.ToLChab(rgbColor);
        }

        /// <summary>
        /// Converts LCH colour back to System.Windows.Media.Color.
        /// </summary>
        public static Color ToColor(this LChabColor lchColor)
        {
            RGBColor rgbColor = _converter.ToRGB(lchColor);

            byte R = (byte) Math.Round(rgbColor.R * 255.0, MidpointRounding.AwayFromZero);
            byte G = (byte) Math.Round(rgbColor.G * 255.0, MidpointRounding.AwayFromZero);
            byte B = (byte) Math.Round(rgbColor.B * 255.0, MidpointRounding.AwayFromZero);
            
            return Color.FromArgb(255, R, G, B);
        }

        /// <summary>
        /// Converts a collection of LCH colours to Colo.
        /// </summary>
        public static List<Color> ToColor(IEnumerable<LChabColor> lchColors)
        {
            List<Color> colorList = new List<Color>();
            
            foreach (var colorLch in lchColors)
                colorList.Add(ToColor(colorLch));

            return colorList;
        }

        /// <summary>
        /// Converts a list of colours to LCH.
        /// </summary>
        public static List<LChabColor> ToLch(IEnumerable<Color> colorList)
        {
            List<LChabColor> lchList = new List<LChabColor>();
            foreach (Color color in colorList)
                lchList.Add(ToLch(color));

            return lchList;
        }
    }
}
