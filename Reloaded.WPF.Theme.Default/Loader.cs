using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Reloaded.WPF.Theme.Default
{
    /// <summary>
    /// Create an instance of this class to load the current theme to this specific window/application or <see cref="FrameworkElement"/>.
    /// </summary>
    public static class Loader
    {
        /* Location of Font Set and XAML Styles */
        public const string RelativeRootXamlFileLocation    = "Theme/Default/Root.xaml";
        public const string RelativeFontLocation            = "Theme/Default/Fonts";

        public static void Load(FrameworkElement element)
        {
            LoadFonts(new ResourceManipulator(element));
            LoadStyle(element);
        }

        /// <summary>
        /// Loads all OTF and TTF fonts by file names matching their resource
        /// </summary>
        private static void LoadFonts(ResourceManipulator resources)
        {
            string fontDirectory = AppDomain.CurrentDomain.BaseDirectory + RelativeFontLocation;
            if (Directory.Exists(fontDirectory))
            {
                List<string> fontFiles = Directory.GetFiles(fontDirectory, "*.otf").ToList();
                fontFiles.AddRange(Directory.GetFiles(fontDirectory, "*.ttf"));

                foreach (var fontFileLocation in fontFiles)
                {
                    try
                    {
                        foreach (var fontFamily in Fonts.GetFontFamilies(fontFileLocation))
                        {
                            resources.Set(Path.GetFileNameWithoutExtension(fontFileLocation), fontFamily);
                        }
                    }
                    catch { /* Ignored*/ }
                }
            }
        }

        /// <summary>
        /// Loads a custom WPF style from the filesystem.
        /// </summary>
        private static void LoadStyle(FrameworkElement element)
        {
            string themeRootXamlFile = AppDomain.CurrentDomain.BaseDirectory + RelativeRootXamlFileLocation;
            if (File.Exists(themeRootXamlFile))
                element.Resources.MergedDictionaries.Add(
                    new ResourceDictionary() { Source = new Uri(themeRootXamlFile, UriKind.Absolute) });
        }
    }
}
