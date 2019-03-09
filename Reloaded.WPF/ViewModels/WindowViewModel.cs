using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using PropertyChanged;
using Reloaded.WPF.Utilities;
using Reloaded.WPF.ViewModels.Base;
using Reloaded.WPF.ViewModels.Helpers;
using static Reloaded.WPF.ViewModels.Helpers.ResourceManipulator;

namespace Reloaded.WPF.ViewModels
{
    public class WindowViewModel : BaseViewModel, IDisposable
    {
        #region XAML Key Names
        // ReSharper disable InconsistentNaming
        private const string XAML_DropShadowBorderName  = "DropShadowBorder";
        private const string XAML_DropShadowColor       = "DropShadowColor";
        private const string XAML_CornerRadius          = "CornerRadius";
        private const string XAML_TitleBarHeight        = "TitleBarHeight";
        
        private const string XAML_BorderlessOnDock      = "BorderlessOnDock";
        private const string XAML_DropShadowSize        = "DropShadowSize";
        private const string XAML_DropShadowBorderSize  = "DropShadowBorderSize";
        private const string XAML_ResizeBorderThickness = "ResizeBorderThickness";

        private const string XAML_EnableGlow            = "EnableGlow";
        private const string XAML_GlowOpacity           = "GlowOpacity";
        private const string XAML_GlowDirection         = "GlowDirection";
        private const string XAML_GlowDepth             = "GlowDepth";

        private const string XAML_DefaultMinWidth       = "DefaultMinWidth";
        private const string XAML_DefaultMinHeight      = "DefaultMinHeight";
        private const string XAML_CloseButtonVisibility     = "CloseButtonVisibility";
        private const string XAML_MinimizeButtonVisibility  = "MinimizeButtonVisibility";
        private const string XAML_MaximizeButtonVisibility  = "MaximizeButtonVisibility";

        private const string XAML_EnableGlowHueCycle          = "EnableGlowHueCycle";
        private const string XAML_GlowHueCycleFramesPerSecond = "GlowHueCycleFramesPerSecond";
        private const string XAML_GlowHueCycleLoopDuration    = "GlowHueCycleLoopDuration";
        private const string XAML_GlowHueCycleChroma          = "GlowHueCycleChroma";
        private const string XAML_GlowHueCycleLightness       = "GlowHueCycleLightness";
        // ReSharper restore InconsistentNaming
        #endregion

        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        
        private ResourceManipulator     _resources;
        private Window                  _targetWindow;
        private WindowResizer           _windowResizer;
        private Effect                  _oldDropShadowEffect;

        private Thread              _cycleDropShadow;
        private WindowDockPosition  _dockPosition = WindowDockPosition.Undocked;

        /* Note: All sizes are in points, not pixels. */
        public WindowViewModel(Window window)
        {
            // Note that we are breaking MVVM concepts by accepting this parameter,
            // though this is not a viewmodel we will probably use in non-desktop
            // environments and everything here is specific to desktop style.
            _targetWindow  = window;
            _resources     = new ResourceManipulator(window);
            _windowResizer = new WindowResizer(_targetWindow);

            LoadFonts();

            // Notify drop shadow/border on change of state
            // or window dock position.
            _targetWindow.StateChanged += (sender, args) => GlowStateChanged();
            _windowResizer.WindowDockChanged += position =>
            {
                _dockPosition = position;
                GlowStateChanged();
            };

            // Update clip area (ensure titlebar background rounded corner) when
            // the window is resized.
            _targetWindow.SizeChanged += (sender, args) =>
            {
                if (RealHeight > 0 && RealWidth > 0)
                    ClientArea = new Rect(0, 0, RealWidth, RealHeight);
            };

            // Enable/Disable Glow based off of XAML preferences once
            // the window fully loads.
            _targetWindow.Loaded += (sender, args) =>
            {
                EnableGlow = EnableGlow;
            };

            // Set initial minimum window sizes if not defined.
            if (Math.Abs(_targetWindow.MinHeight) < 0.1F)
                _targetWindow.MinHeight = _resources.Get<double>(XAML_DefaultMinHeight);

            if (Math.Abs(_targetWindow.MinWidth) < 0.1F)
                _targetWindow.MinWidth  = _resources.Get<double>(XAML_DefaultMinWidth);

            // Implement Titlebar Buttons
            MinimizeCommand = new ActionCommand(() => { _targetWindow.WindowState = WindowState.Minimized;  });
            MaximizeCommand = new ActionCommand(() => { _targetWindow.WindowState ^= WindowState.Maximized; });
            CloseCommand    = new ActionCommand(() => { this.Dispose(); _targetWindow.Close(); });

            // Fun
            if (_resources.Get<bool>(XAML_EnableGlowHueCycle))
                EnableHueCycleDropShadow(
                    _resources.Get<int>(XAML_GlowHueCycleFramesPerSecond),
                    _resources.Get<int>(XAML_GlowHueCycleLoopDuration),
                    _resources.Get<float>(XAML_GlowHueCycleChroma),
                    _resources.Get<float>(XAML_GlowHueCycleLightness));
        }

        /* Public Tweakables */

        /// <summary>
        /// The size of the invisible border that provides the WPF window space
        /// to draw the drop shadow.
        /// </summary>
        public Thickness DropShadowBorderSize
        {
            get
            {
                if (EnableGlow)
                {
                    return IsMaximized()  ? _windowResizer.CurrentMonitorMargin
                                          : (IsBorderlessOnDock()
                                            ? new Thickness(0)
                                            : _resources.Get<Thickness>(XAML_DropShadowBorderSize));
                }
                
                return new Thickness(0);
            }
            set => _resources.Set( XAML_DropShadowBorderSize, value);
        }

        /// <summary>
        /// The size of the drop shadow present within the invisible border of the form.
        /// </summary>
        public double DropShadowSize
        {
            get
            {
                if (EnableGlow)
                    return IsBorderlessOnDock() ? 0 
                                                : _resources.Get<double>(XAML_DropShadowSize);

                return 0;
            }
            set => _resources.Set(XAML_DropShadowSize, value);
        }

        /// <summary>
        /// Thickness of the border that allows you to resize the window.
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get
            {
                if (EnableGlow)
                {
                    return IsBorderlessOnDock()
                        ? XamlResizeBorderThickness
                        : new Thickness(DropShadowBorderSize.Left + XamlResizeBorderThickness.Left,
                            DropShadowBorderSize.Top + XamlResizeBorderThickness.Top,
                            DropShadowBorderSize.Right + XamlResizeBorderThickness.Right,
                            DropShadowBorderSize.Bottom + XamlResizeBorderThickness.Bottom);
                }

                return XamlResizeBorderThickness;
            }
            set => XamlResizeBorderThickness = value;
        }

        /// <summary>
        /// The colour of the drop shadow (glow) effect around the window.
        /// </summary>
        public Color DropShadowColor
        {
            get => _resources.Get<Color>(XAML_DropShadowColor);
            set => _resources.Set(XAML_DropShadowColor, value);
        }

        /// <summary>
        /// Radius of the rounded corners of the window.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => _resources.Get<CornerRadius>(XAML_CornerRadius);
            set => _resources.Set(XAML_CornerRadius, value);
        }

        /// <summary>
        /// Height of the Title Bar at the top of the window.
        /// </summary>
        public GridLength TitleBarHeight
        {
            get => _resources.Get<GridLength>(XAML_TitleBarHeight);
            set => _resources.Set(XAML_TitleBarHeight, value);
        }

        /// <summary>
        /// The opacity of the drawn drop shadow/glow effect.
        /// </summary>
        public double GlowOpacity
        {
            get => _resources.Get<double>(XAML_GlowOpacity);
            set => _resources.Set(XAML_GlowOpacity, value);
        }

        /// <summary>
        /// The opacity of the drawn drop shadow/glow effect.
        /// </summary>
        public double GlowDirection
        {
            get => _resources.Get<double>(XAML_GlowDirection);
            set => _resources.Set(XAML_GlowDirection, value);
        }

        /// <summary>
        /// The opacity of the drawn drop shadow/glow effect.
        /// </summary>
        public double GlowDepth
        {
            get => _resources.Get<double>(XAML_GlowDepth);
            set => _resources.Set(XAML_GlowDepth, value);
        }

        /// <summary>
        /// If this is set to true, the window will go borderless
        /// if it is maximized or docked.
        /// </summary>
        public bool BorderlessOnDock
        {
            get => _resources.Get<bool>(XAML_BorderlessOnDock);
            set => _resources.Set(XAML_BorderlessOnDock, value);
        }

        /// <summary>
        /// Gets/Sets the visibility of the minimize titlebar button.
        /// </summary>
        public Visibility MinimizeButtonVisibility
        {
            get => _resources.Get<Visibility>(XAML_MinimizeButtonVisibility);
            set => _resources.Set(XAML_MinimizeButtonVisibility, value);
        }

        /// <summary>
        /// Gets/Sets the visibility of the maximize titlebar button.
        /// </summary>
        public Visibility MaximizeButtonVisibility
        {
            get => _resources.Get<Visibility>(XAML_MaximizeButtonVisibility);
            set => _resources.Set(XAML_MaximizeButtonVisibility, value);
        }

        /// <summary>
        /// Gets/Sets the visibility of the close titlebar button.
        /// </summary>
        public Visibility CloseButtonVisibility
        {
            get => _resources.Get<Visibility>(XAML_CloseButtonVisibility);
            set => _resources.Set(XAML_CloseButtonVisibility, value);
        }

        /// <summary>
        /// Enables or disables the glow/drop shadow effect.
        /// </summary>
        [DoNotCheckEquality]
        public bool EnableGlow
        {
            get => _resources.Get<bool>(XAML_EnableGlow);
            set
            {
                if (value == false)
                    DisableDropShadow();
                else
                    EnableDropShadow();

                _resources.Set(XAML_EnableGlow, value);
                GlowStateChanged();
            }
        }

        /* Fun */

        /// <summary>
        /// Enables hue cycling of the drop shadow for the window.
        /// </summary>
        /// <param name="framesPerSecond">The amount of frames per second.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        /// <param name="chroma">Range 0 to 100. The quality of a color's purity, intensity or saturation. </param>
        /// <param name="lightness">Range 0 to 100. The quality (chroma) lightness or darkness.</param>
        /// <remarks>https://www.harding.edu/gclayton/color/topics/001_huevaluechroma.html</remarks>
        public void EnableHueCycleDropShadow(int framesPerSecond = 30, int duration = 6000, float chroma = 50F, float lightness = 50F)
        {
            _cycleDropShadow = Fun.HueCycleColor(color =>
            {
                if (_cycleDropShadow != null)
                    DropShadowColor = color;
            }, framesPerSecond, duration, chroma, lightness);

            _cycleDropShadow.IsBackground = true;
        }

        /// <summary>
        /// Disables hue cycling of the drop shadow for the window.
        /// </summary>
        public void DisableHueCycleDropShadow()
        {
            _cycleDropShadow = null;
        }

        /* Rendering & Control Shenanigans */

        /// <summary>
        /// Contains the real size of the client area of the window, i.e. the area the user interacts.
        /// This is defined as the window width,height minus the invisible border size.
        /// </summary>
        public Rect ClientArea { get; private set; } = new Rect(0, 0, 560, 860);

        /// <summary>
        /// The actual width of the internal contents of the window excluding the invisible drop shadow border.
        /// </summary>
        public int RealWidth => (int)_targetWindow.ActualWidth - (2 * (int)DropShadowBorderSize.Left);

        /// <summary>
        /// The actual height of the internal contents of the window excluding the invisible drop shadow border.
        /// </summary>
        public int RealHeight => (int)_targetWindow.ActualHeight - (2 * (int)DropShadowBorderSize.Left);

        /// <summary>
        /// [For WindowChrome]
        /// Gets the real titlebar height of the window, including the drop shadow border.
        /// </summary>
        public int WindowChromeTitleBarHeight => (int)(TitleBarHeight.Value - XamlResizeBorderThickness.Top + _windowResizer.CurrentMonitorMargin.Top);

        private Thickness XamlResizeBorderThickness
        {
            get => _resources.Get<Thickness>(XAML_ResizeBorderThickness);
            set => _resources.Set(XAML_ResizeBorderThickness, value);
        }

        /* Core Logic */
        private void DisableDropShadow()
        {
            _targetWindow.Dispatcher.Invoke(() =>
            {
                Border _oldBorder  = (Border)_targetWindow.Template.FindName(XAML_DropShadowBorderName, _targetWindow);
                if (_oldBorder != null)
                {
                    _oldDropShadowEffect = _oldBorder.Effect;
                    _oldBorder.Effect = null;
                }
            });
        }

        private void EnableDropShadow()
        {
            _targetWindow.Dispatcher.Invoke(() =>
            {
                Border _oldBorder = (Border)_targetWindow.Template.FindName(XAML_DropShadowBorderName, _targetWindow);
                if (_oldBorder != null && _oldDropShadowEffect != null)
                    _oldBorder.Effect = _oldDropShadowEffect;
            });
        }

        /// <summary>
        /// The event to raise to inform of the possible change of values of other properties
        /// when the glow state of the form may change.
        /// </summary>
        private void GlowStateChanged()
        {
            NotifyPropertyChanged(nameof(DropShadowBorderSize));
            NotifyPropertyChanged(nameof(DropShadowSize));
            NotifyPropertyChanged(nameof(ResizeBorderThickness));
            NotifyPropertyChanged(nameof(WindowChromeTitleBarHeight));
        }

        /* Helpers */

        private bool IsBorderlessOnDock()
        {
            return BorderlessOnDock &&  
                  (IsMaximized() ||
                   _dockPosition != WindowDockPosition.Undocked);
        }

        private bool IsMaximized()
        {
            return _targetWindow.WindowState == WindowState.Maximized;
        }

        public void Dispose()
        {
            _cycleDropShadow = null;
        }

        /// <summary>
        /// Loads all OTF and TTF fonts by file names matching their resource
        /// </summary>
        private void LoadFonts()
        {
            string fontDirectory = AppDomain.CurrentDomain.BaseDirectory + "Theme/Fonts";
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
                            _resources.Set(Path.GetFileNameWithoutExtension(fontFileLocation), fontFamily);
                        }
                    }
                    catch { /* Ignored*/ }
                }
            }
        }
    }
}
