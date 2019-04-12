﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using PropertyChanged;
using Reloaded.WPF.Pages;
using Reloaded.WPF.Utilities;
using Reloaded.WPF.Utilities.Animation.Manual;

namespace Reloaded.WPF.Theme.Default
{
    public class WindowViewModel : BaseWindowViewModel, IDisposable
    {
        #region XAML Key Names
        // ReSharper disable InconsistentNaming
        private const string XAML_DropShadowBorderName  = "DropShadowBorder";
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

        private const string XAML_AllowGlowStateChange  = "AllowGlowStateChange";
        private const string XAML_GlowColorInactive     = "GlowColorInactive";
        private const string XAML_GlowColorDefault      = "GlowColorDefault";
        private const string XAML_GlowColorEngaged      = "GlowColorEngaged";
        private const string XAML_GlowColorAnimationEnable   = "GlowColorAnimationEnable";
        private const string XAML_GlowColorAnimationDuration = "GlowColorAnimationDuration";
        private const string XAML_GlowColorAnimationFramesPerSecond = "GlowColorAnimationFramesPerSecond";

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

        private State _windowState = State.Normal;
        private Effect _oldDropShadowEffect;

        private CancellationTokenSource _hueCycleDropShadowToken;
        private CancellationTokenSource _glowColorAnimateToken;
        private Task _glowColorAnimateTask;
        private Task _hueCycleDropShadowTask;

        /* Note: All sizes are in points, not pixels. */
        public WindowViewModel(Window window) : base(window)
        {
            // Load window style.
            Loader.Load(window);

            // Notify drop shadow/border on change of state
            // or window dock position.
            TargetWindow.StateChanged += (sender, args) => GlowStateChanged();
            WindowDockChanged += position => DockPosition = position;

            // Enable/Disable Glow based off of XAML preferences once
            // the window fully loads.
            TargetWindow.Loaded += (sender, args) =>
            {
                EnableGlow = EnableGlow;
            };

            // Set initial minimum window sizes if not defined.
            if (Math.Abs(TargetWindow.MinHeight) < 0.1F)
                TargetWindow.MinHeight = Resources.Get<double>(XAML_DefaultMinHeight);

            if (Math.Abs(TargetWindow.MinWidth) < 0.1F)
                TargetWindow.MinWidth  = Resources.Get<double>(XAML_DefaultMinWidth);

            // Handle window out of focus.
            TargetWindow.Deactivated += (sender, args) => SetGlowColor(GetGlowColorFromState());
            TargetWindow.Activated += (sender, args) => SetGlowColor(GetGlowColorFromState()); 

            // Fun
            if (Resources.Get<bool>(XAML_EnableGlowHueCycle))
            {
                EnableHueCycleDropShadow(
                    Resources.Get<int>(XAML_GlowHueCycleFramesPerSecond),
                    Resources.Get<int>(XAML_GlowHueCycleLoopDuration),
                    Resources.Get<float>(XAML_GlowHueCycleChroma),
                    Resources.Get<float>(XAML_GlowHueCycleLightness));
            }
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
                    return IsMaximized()  ? WindowResizer.CurrentMonitorMargin
                                          : (IsBorderlessOnDock()
                                            ? new Thickness(0)
                                            : Resources.Get<Thickness>(XAML_DropShadowBorderSize));
                }
                
                return new Thickness(0);
            }
            set => Resources.Set( XAML_DropShadowBorderSize, value);
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
                                                : Resources.Get<double>(XAML_DropShadowSize);

                return 0;
            }
            set => Resources.Set(XAML_DropShadowSize, value);
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
        /// The active colour of the drop shadow (glow) effect around the window.
        /// Note: Setting this will not animate the transition, if you would like for the transition to be animated
        /// make sure that <see cref="GlowColorAnimationEnable"/> is set to "true" and that you use <see cref="SetGlowColor"/> instead.
        /// </summary>
        public Color GlowColor { get; set; }

        /// <summary>
        /// Allows or disallows the active changing of window colour based on whether
        /// the window is busy.
        /// </summary>
        public bool AllowGlowStateChange
        {
            get => Resources.Get<bool>(XAML_AllowGlowStateChange);
            set => Resources.Set(XAML_AllowGlowStateChange, value);
        }

        /// <summary>
        /// The colour of the drop shadow (glow) effect around the window
        /// when the window is out of focus.
        /// </summary>
        public Color GlowColorInactive
        {
            get => Resources.Get<Color>(XAML_GlowColorInactive);
            set => Resources.Set(XAML_GlowColorInactive, value);
        }

        /// <summary>
        /// The colour of the drop shadow (glow) effect around the window
        /// when the window is working.
        /// </summary>
        public Color GlowColorEngaged
        {
            get => Resources.Get<Color>(XAML_GlowColorEngaged);
            set => Resources.Set(XAML_GlowColorEngaged, value);
        }

        /// <summary>
        /// The colour of the drop shadow (glow) effect around the window
        /// when the window is out of focus.
        /// </summary>
        public Color GlowColorDefault
        {
            get => Resources.Get<Color>(XAML_GlowColorDefault);
            set => Resources.Set(XAML_GlowColorDefault, value);
        }

        /// <summary>
        /// If true, animates the switches between the different window states.
        /// </summary>
        public bool GlowColorAnimationEnable
        {
            get => Resources.Get<bool>(XAML_GlowColorAnimationEnable);
            set => Resources.Set(XAML_GlowColorAnimationEnable, value);
        }

        /// <summary>
        /// The duration of the border glow transition between the different Window <see cref="State"/>s.
        /// </summary>
        public int GlowColorAnimationDuration
        {
            get => Resources.Get<int>(XAML_GlowColorAnimationDuration);
            set => Resources.Set(XAML_GlowColorAnimationDuration, value);
        }

        /// <summary>
        /// If true, animates the switches between the different window states.
        /// </summary>
        public int GlowColorAnimationFramesPerSecond
        {
            get => Resources.Get<int>(XAML_GlowColorAnimationFramesPerSecond);
            set => Resources.Set(XAML_GlowColorAnimationFramesPerSecond, value);
        }

        /// <summary>
        /// Radius of the rounded corners of the window.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => Resources.Get<CornerRadius>(XAML_CornerRadius);
            set => Resources.Set(XAML_CornerRadius, value);
        }

        /// <summary>
        /// Height of the Title Bar at the top of the window.
        /// </summary>
        public GridLength TitleBarHeight
        {
            get => Resources.Get<GridLength>(XAML_TitleBarHeight);
            set => Resources.Set(XAML_TitleBarHeight, value);
        }

        /// <summary>
        /// The opacity of the drawn drop shadow/glow effect.
        /// </summary>
        public double GlowOpacity
        {
            get => Resources.Get<double>(XAML_GlowOpacity);
            set => Resources.Set(XAML_GlowOpacity, value);
        }

        /// <summary>
        /// The opacity of the drawn drop shadow/glow effect.
        /// </summary>
        public double GlowDirection
        {
            get => Resources.Get<double>(XAML_GlowDirection);
            set => Resources.Set(XAML_GlowDirection, value);
        }

        /// <summary>
        /// The opacity of the drawn drop shadow/glow effect.
        /// </summary>
        public double GlowDepth
        {
            get => Resources.Get<double>(XAML_GlowDepth);
            set => Resources.Set(XAML_GlowDepth, value);
        }

        /// <summary>
        /// If this is set to true, the window will go borderless
        /// if it is maximized or docked.
        /// </summary>
        public bool BorderlessOnDock
        {
            get => Resources.Get<bool>(XAML_BorderlessOnDock);
            set => Resources.Set(XAML_BorderlessOnDock, value);
        }

        /// <summary>
        /// Gets/Sets the visibility of the minimize titlebar button.
        /// </summary>
        public Visibility MinimizeButtonVisibility
        {
            get => Resources.Get<Visibility>(XAML_MinimizeButtonVisibility);
            set => Resources.Set(XAML_MinimizeButtonVisibility, value);
        }

        /// <summary>
        /// Gets/Sets the visibility of the maximize titlebar button.
        /// </summary>
        public Visibility MaximizeButtonVisibility
        {
            get => Resources.Get<Visibility>(XAML_MaximizeButtonVisibility);
            set => Resources.Set(XAML_MaximizeButtonVisibility, value);
        }

        /// <summary>
        /// Gets/Sets the visibility of the close titlebar button.
        /// </summary>
        public Visibility CloseButtonVisibility
        {
            get => Resources.Get<Visibility>(XAML_CloseButtonVisibility);
            set => Resources.Set(XAML_CloseButtonVisibility, value);
        }

        /// <summary>
        /// Enables or disables the glow/drop shadow effect.
        /// </summary>
        public bool EnableGlow
        {
            get => Resources.Get<bool>(XAML_EnableGlow);
            set
            {
                if (value == false)
                    DisableDropShadow();
                else
                    EnableDropShadow();

                Resources.Set(XAML_EnableGlow, value);
                GlowStateChanged();
            }
        }

        /// <summary>
        /// Defines of the state of the window, which in turn changes the glow colour.
        /// </summary>
        public State WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                SetGlowColor(GetGlowColorFromState());
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
        public async void EnableHueCycleDropShadow(int framesPerSecond = 30, int duration = 6000, float chroma = 50F,
            float lightness = 50F)
        {
            AllowGlowStateChange = false;

            if (_hueCycleDropShadowToken != null)
            {
                _hueCycleDropShadowToken.Cancel();
                await _hueCycleDropShadowTask;
            }

            _hueCycleDropShadowToken = new CancellationTokenSource();
            _hueCycleDropShadowTask = Fun.HueCycleColor(color => GlowColor = color, _hueCycleDropShadowToken.Token, framesPerSecond, duration, chroma, lightness);
        }

        /// <summary>
        /// Disables hue cycling of the drop shadow for the window.
        /// </summary>
        public void DisableHueCycleDropShadow()
        {
            _hueCycleDropShadowToken.Cancel();
            _hueCycleDropShadowTask.Wait();
            AllowGlowStateChange = true;
        }

        /* Rendering & Control Shenanigans */

        /// <summary>
        /// [For WindowChrome]
        /// Gets the real titlebar height of the window, including the drop shadow border.
        /// </summary>
        public int WindowChromeTitleBarHeight
        {
            get
            {
                if (IsMaximized())
                {
                    return (int)(TitleBarHeight.Value - XamlResizeBorderThickness.Top +
                                 WindowResizer.CurrentMonitorMargin.Top);
                }

                return (int) (TitleBarHeight.Value - XamlResizeBorderThickness.Top);
            }
        }

        private Thickness XamlResizeBorderThickness
        {
            get => Resources.Get<Thickness>(XAML_ResizeBorderThickness);
            set => Resources.Set(XAML_ResizeBorderThickness, value);
        }

        /* Core Logic */


        /// <summary>
        /// Sets a new glow colour for the window.
        /// The change will be animated if <see cref="GlowColorAnimationEnable"/> is set to "true".
        /// </summary>
        /// <param name="newColor">The new glow colour.</param>
        public async void SetGlowColor(Color newColor)
        {
            Color currentColor = GlowColor;

            if (currentColor != newColor)
            {
                if (GlowColorAnimationEnable)
                {
                    if (_glowColorAnimateToken != null)
                    {
                        _glowColorAnimateToken.Cancel();
                        await _glowColorAnimateTask;
                    }

                    _glowColorAnimateToken = new CancellationTokenSource();
                    _glowColorAnimateTask = Fun.ColorAnimate(x => GlowColor = x, _glowColorAnimateToken.Token, currentColor, newColor, GlowColorAnimationFramesPerSecond, GlowColorAnimationDuration);
                }

                else
                    GlowColor = newColor;
            }
        }

        /// <summary>
        /// Retrieves the new/next glow colour for when the state of the window changes.
        /// </summary>
        private Color GetGlowColorFromState()
        {
            if (AllowGlowStateChange)
            {
                if (TargetWindow.IsActive)
                {
                    switch (_windowState)
                    {
                        case State.Normal: return GlowColorDefault;
                        case State.Engaged: return GlowColorEngaged;
                    }
                }

                return GlowColorInactive;
            }

            return GlowColor;
        }

        private void DisableDropShadow()
        {
            TargetWindow.Dispatcher.Invoke(() =>
            {
                Border _oldBorder  = (Border)TargetWindow.Template.FindName(XAML_DropShadowBorderName, TargetWindow);
                if (_oldBorder != null)
                {
                    _oldDropShadowEffect = _oldBorder.Effect;
                    _oldBorder.Effect = null;
                }
            });
        }

        private void EnableDropShadow()
        {
            TargetWindow.Dispatcher.Invoke(() =>
            {
                Border _oldBorder = (Border)TargetWindow.Template.FindName(XAML_DropShadowBorderName, TargetWindow);
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
            RaisePropertyChangedEvent(nameof(DropShadowBorderSize));
            RaisePropertyChangedEvent(nameof(DropShadowSize));
            RaisePropertyChangedEvent(nameof(ResizeBorderThickness));
            RaisePropertyChangedEvent(nameof(WindowChromeTitleBarHeight));
        }

        /* Helpers */

        private bool IsBorderlessOnDock()
        {
            return BorderlessOnDock &&  
                  (IsMaximized() ||
                   DockPosition != WindowDockPosition.Undocked);
        }

        private bool IsMaximized()
        {
            return TargetWindow.WindowState == System.Windows.WindowState.Maximized;
        }

        public void Dispose()
        {
            
        }

        /* Other classes */

        /// <summary>
        /// Describes the current state of the window.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Nothing out of the order is going on.
            /// </summary>
            Normal,

            /// <summary>
            /// Some kind of work is being done.
            /// </summary>
            Engaged
        }
    }
}
