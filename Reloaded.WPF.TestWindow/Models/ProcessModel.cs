using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Reloaded.WPF.TestWindow.Models
{
    public class ProcessModel
    {
        /// <summary> The ID of this individual process. </summary>
        private System.Diagnostics.Process _process;

        /// <summary> A persistent cache of all the icons of executables. </summary>
        private static Dictionary<String, BitmapSource> _iconCache = new Dictionary<string, BitmapSource>();

        /// <summary>
        /// Creates a process given a process ID.
        /// </summary>
        /// <param name="process">The individual process instance representing the process.</param>
        public ProcessModel(Process process)
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
                catch (Exception e) { return "";  }
            }
        }

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
                catch (Exception ex)
                {
                    string defaultIconPath = "pack://application:,,,/Reloaded.WPF;Component/Images/Reloaded_Icon.png";

                    // Try to retrieve from cache.
                    if (_iconCache.TryGetValue(defaultIconPath, out BitmapSource icon))
                        return icon;

                    var image = new BitmapImage(new Uri(defaultIconPath, UriKind.RelativeOrAbsolute));
                    var cachedBitmap = new CachedBitmap(image, BitmapCreateOptions.None, BitmapCacheOption.Default);
                    cachedBitmap.Freeze();

                    _iconCache[defaultIconPath] = cachedBitmap;
                    return image;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
