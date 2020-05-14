using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Reloaded.WPF.Controls
{
    /// <summary>
    /// A variation of <see cref="ResourceDictionary"/> that loads all fonts in a given directory,
    /// relative to application folder assigning the name of the font file as the key.
    /// </summary>
    public class FontFamilyResourceDictionary : ResourceDictionary, INotifyPropertyChanged
    {
        /// <summary>
        /// The source where the font originates from.
        /// </summary>
        public new string Source { get; set; }

        public FontFamilyResourceDictionary()
        {
            this.PropertyChanged += SetFontURI;
        }

        private void SetFontURI(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Source))
                UpdateSource();
        }

        private void UpdateSource()
        {
            if (Source != null)
                LoadFonts();
        }

        private void LoadFonts()
        {
            this.Clear();
            var fontDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/{Source}";
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
                            this[Path.GetFileNameWithoutExtension(fontFileLocation)] = fontFamily;
                        }
                    }
                    catch { /* Ignored*/ }
                }
            }
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
