using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Reloaded.WPF.Resources;

namespace Reloaded.WPF.TestWindow.Models.Model
{
    public class Process
    {
        /// <summary> The ID of this individual process. </summary>
        private System.Diagnostics.Process _process;

        /// <summary> A persistent cache of all the icons of executables. </summary>
        private static Dictionary<string, BitmapSource> _iconCache = new Dictionary<string, BitmapSource>();

        /// <summary>
        /// Creates a process given a process ID.
        /// </summary>
        /// <param name="process">The individual process instance representing the process.</param>
        public Process(System.Diagnostics.Process process)
        {
            _process = process;
        }

        public string ProcessName
        {
            get
            {
                try
                {
                    return _process.ProcessName;
                }
                catch (Exception) { return "Failed to Get Process Name";  }
            }
        }

        [DebuggerNonUserCode]
        public System.Windows.Media.ImageSource Image 
        {
            get
            {
                try
                {
                    // Try to retrieve from cache.
                    if (_iconCache.TryGetValue(_process.MainModule.FileName, out BitmapSource icon))
                        return icon;
                        
                    // Otherwise make new icon.
                    using (Icon ico = Icon.ExtractAssociatedIcon(_process.MainModule.FileName))
                    {
                        BitmapSource bitmapImage = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        CachedBitmap cachedBitmap = new CachedBitmap(bitmapImage, BitmapCreateOptions.None, BitmapCacheOption.Default);
                        cachedBitmap.Freeze();

                        _iconCache[_process.MainModule.FileName] = cachedBitmap;
                        return bitmapImage;
                    }
                }
                catch (Exception)
                {
                    // Try to retrieve from cache.
                    if (_iconCache.TryGetValue(Paths.PLACEHOLDER_IMAGE, out BitmapSource icon))
                        return icon;

                    var image = new BitmapImage(new Uri(Paths.PLACEHOLDER_IMAGE, UriKind.RelativeOrAbsolute));
                    var cachedBitmap = new CachedBitmap(image, BitmapCreateOptions.None, BitmapCacheOption.Default);
                    cachedBitmap.Freeze();

                    _iconCache[Paths.PLACEHOLDER_IMAGE] = cachedBitmap;
                    return image;
                }
            }
        }
    }
}
