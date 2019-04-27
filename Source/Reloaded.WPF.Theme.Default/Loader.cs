using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Reloaded.WPF.Theme.Default
{
    /// <summary>
    /// Create an instance of this class to load the current theme to this specific window/application or <see cref="FrameworkElement"/>.
    /// </summary>
    public static class Loader
    {
        /// <summary> Shared instance of the load once dictionary. </summary>
        private static ResourceDictionary _dictionary;

        /* Location of Font Set and XAML Styles */
        private const string RelativeRootXamlFileLocation    = "Theme/Default/Root.xaml";
        private const string RelativeFontLocation            = "Theme/Default/Fonts";

        /// <summary>
        /// Loads the Reloaded.WPF theme into the given Windows Presentation Foundation Framework element.
        /// </summary>
        public static void Load(FrameworkElement element)
        {
            // Instantiate dictionary if not created.
            if (_dictionary == null)
            {
                _dictionary = new ResourceDictionary();
                LoadStyle();
                LoadFonts();
            }

            // Merge the dictionary in.
            element.Resources.MergedDictionaries.Add(_dictionary);
        }

        /// <summary>
        /// Loads all OTF and TTF fonts by file names matching their resource
        /// </summary>
        private static void LoadFonts()
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
                            _dictionary[Path.GetFileNameWithoutExtension(fontFileLocation)] = fontFamily;
                        }
                    }
                    catch { /* Ignored*/ }
                }
            }
        }

        /// <summary>
        /// Loads a custom WPF style from the filesystem.
        /// </summary>
        private static void LoadStyle()
        {
            string themeRootXamlFile = AppDomain.CurrentDomain.BaseDirectory + RelativeRootXamlFileLocation;
            if (File.Exists(themeRootXamlFile))
                _dictionary.MergedDictionaries.Add( new ResourceDictionary() { Source = new Uri(themeRootXamlFile, UriKind.Absolute) } );
        }
    }
}
