#pragma warning disable 1591

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using PropertyChanged;
using Reloaded.WPF.Animations.Samples;
using Reloaded.WPF.Pages;
using Reloaded.WPF.Utilities;
using Color = System.Windows.Media.Color;

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
        private const string XAML_IgnoreInactiveStateWhenSpecialState = "IgnoreInactiveStateWhenSpecialState";
        private const string XAML_IgnoreInactiveState = "IgnoreInactiveState";

        private const string XAML_DefaultMinWidth       = "DefaultMinWidth";
        private const string XAML_DefaultMinHeight      = "DefaultMinHeight";
        private const string XAML_CloseButtonVisibility     = "CloseButtonVisibility";
        private const string XAML_MinimizeButtonVisibility  = "MinimizeButtonVisibility";
        private const string XAML_MaximizeButtonVisibility  = "MaximizeButtonVisibility";

        private const string XAML_EnableGlowHueCycle          = "EnableGlowHueCycle";
        private const string XAML_GlowHueCycleFramesPerSecond = "GlowHueCycleFramesPerSecond";
        private const string XAML_GlowHueCycleLoopDuration    = "GlowHueCycleLoopDuration";
        private const string XAML_GlowHueCycleColor           = "GlowHueCycleColor";
        // ReSharper restore InconsistentNaming
        #endregion

        private State _windowState = State.Normal;
        private Effect _oldDropShadowEffect;

        private DictionaryResourceManipulator Resources { get; set; }
        private LerpColorAnimation _glowColorAnimation;
        private CycleColorAnimation _hueCycleDropShadowAnimation;

        private State _lastState;
        private bool _isClosed = false;
        private Window _window;

        /* Note: All sizes are in points, not pixels. */

        public WindowViewModel(Window window) : base(window)
        {
            // Load window style.
            Resources = new DictionaryResourceManipulator(Application.Current.Resources);
            _window = window;

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

            // Set default glow colour.
            GlowColor = GlowColorDefault;

            // Handle window state changes.
            TargetWindow.Deactivated += (sender, args) =>
            {
                if (!IgnoreInactiveState)
                {
                    this._lastState = this.WindowState;

                    // Ignore if special state and flag set.
                    if (IgnoreInactiveStateWhenSpecialState && WindowState != State.Normal)
                        return;

                    this.WindowState = State.Inactive;
                }
            };

            TargetWindow.Activated += (sender, args)   =>
            {
                if (this._lastState == State.Inactive)
                    this.WindowState = State.Normal;
                else
                    this.WindowState = this._lastState;
            }; 

            // Fun
            if (Resources.Get<bool>(XAML_EnableGlowHueCycle))
            {
                EnableHueCycleDropShadow(
                    Resources.Get<Color>(XAML_GlowHueCycleColor),
                    Resources.Get<int>(XAML_GlowHueCycleFramesPerSecond),
                    Resources.Get<int>(XAML_GlowHueCycleLoopDuration));
            }

            window.Closing += WindowOnClosing;

            // Inherit Default from App Resources
            MinimizeButtonVisibility = Resources.Get<Visibility>(XAML_MinimizeButtonVisibility);
            MaximizeButtonVisibility = Resources.Get<Visibility>(XAML_MaximizeButtonVisibility);
            CloseButtonVisibility = Resources.Get<Visibility>(XAML_CloseButtonVisibility);
        }

        private void WindowOnClosing(object sender, CancelEventArgs e)
        {
            _glowColorAnimation?.Pause();
            _hueCycleDropShadowAnimation?.Pause();
            _isClosed = true;
        }

        /* Per Window Settings */

        /// <summary>
        /// Gets/Sets the visibility of the minimize titlebar button.
        /// </summary>
        public Visibility MinimizeButtonVisibility { get; set; }

        /// <summary>
        /// Gets/Sets the visibility of the maximize titlebar button.
        /// </summary>
        public Visibility MaximizeButtonVisibility { get; set; }

        /// <summary>
        /// Gets/Sets the visibility of the close titlebar button.
        /// </summary>
        public Visibility CloseButtonVisibility { get; set; }

        /* Global Settings */

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
            set => Resources.Set(XAML_DropShadowBorderSize, value);
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
        [DoNotNotify]
        public Color GlowColor
        {
            get => (Color) GetValue(GlowColorProperty);
            set => Application.Current.Dispatcher.Invoke((Action<Color>)((color) => SetValue(GlowColorProperty, color)), value);
        }

        // Binding to a DependencyProperty avoids reflection, making it faster for the binding engine to obtain the glow color.
        // This saves CPU times when the glow color is updated at very high frequencies.
        private static DependencyProperty GlowColorProperty = DependencyProperty.Register(nameof(GlowColor), typeof(Color), typeof(WindowViewModel));

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
        /// If true will not set the glow color to <see cref="GlowColorInactive"/> if the current <see cref="WindowState"/> is other than <see cref="State.Normal"/>
        /// </summary>
        public bool IgnoreInactiveStateWhenSpecialState
        {
            get => Resources.Get<bool>(XAML_IgnoreInactiveStateWhenSpecialState);
            set => Resources.Set(XAML_IgnoreInactiveStateWhenSpecialState, value);
        }

        /// <summary>
        /// If true <see cref="State.Inactive"/> (i.e. when the window is out of focus) will not change the border color.
        /// </summary>
        public bool IgnoreInactiveState
        {
            get => Resources.Get<bool>(XAML_IgnoreInactiveState);
            set => Resources.Set(XAML_IgnoreInactiveState, value);
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
        /// <param name="startColor">The colour to start animating from.</param>
        /// <param name="framesPerSecond">The amount of frames per second.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        /// <remarks>https://www.harding.edu/gclayton/color/topics/001_huevaluechroma.html</remarks>
        public void EnableHueCycleDropShadow(Color startColor, int framesPerSecond = 30, int duration = 6000)
        {
            AllowGlowStateChange = false;

            if (_hueCycleDropShadowAnimation == null)
                _hueCycleDropShadowAnimation = new CycleColorAnimation(color => GlowColor = color, startColor, duration, framesPerSecond);

            _hueCycleDropShadowAnimation.Animate();
        }

        /// <summary>
        /// Disables hue cycling of the drop shadow for the window.
        /// </summary>
        public void DisableHueCycleDropShadow()
        {
            _hueCycleDropShadowAnimation.Cancel();
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
        public void SetGlowColor(Color newColor) => SetGlowColor(newColor, false);

        /// <summary>
        /// Sets a new glow colour for the window.
        /// The change will be animated if <see cref="GlowColorAnimationEnable"/> is set to "true".
        /// </summary>
        /// <param name="newColor">The new glow colour.</param>
        /// <param name="setDirectly">Sets the colour directly on the current thread.</param>
        public void SetGlowColor(Color newColor, bool setDirectly = false)
        {
            Color currentColor = GlowColor;

            if (currentColor != newColor)
            {
                if (GlowColorAnimationEnable && !_isClosed)
                {
                    _glowColorAnimation?.CancelAsync();

                    _glowColorAnimation = new LerpColorAnimation(x => GlowColor = x, currentColor, newColor, GlowColorAnimationDuration, GlowColorAnimationFramesPerSecond);
                    _glowColorAnimation.Repeat = 1;
                    _glowColorAnimation.Animate();
                }

                else
                {
                    if (setDirectly)
                        SetValue(GlowColorProperty, newColor);
                    else
                        GlowColor = newColor;
                }
            }
        }

        /// <summary>
        /// Automatically sets the window state to an appropriate value.
        /// This will set the <see cref="WindowState"/> to either a value of <see cref="State.Inactive"/> or <see cref="State.Normal"/>.
        /// </summary>
        public void ResetState()
        {
            if (TargetWindow.IsActive)
                this.WindowState = State.Normal;

            this.WindowState = State.Inactive;
        }

        /// <summary>
        /// Retrieves the new/next glow colour for when the state of the window changes.
        /// </summary>
        private Color GetGlowColorFromState()
        {
            if (AllowGlowStateChange)
            {
                switch (_windowState)
                {
                    case State.Inactive: return GlowColorInactive;
                    case State.Normal:   return GlowColorDefault;
                    case State.Engaged:  return GlowColorEngaged;
                }
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
            _glowColorAnimation?.Dispose();
            _hueCycleDropShadowAnimation?.Dispose();
        }

        /* Other classes */

        /// <summary>
        /// Describes the current state of the window.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// The window is out of focus.
            /// </summary>
            Inactive,

            /// <summary>
            /// The window is in focus but. Nothing out of the order is going on.
            /// </summary>
            Normal,

            /// <summary>
            /// Some kind of work is being done.
            /// </summary>
            Engaged
        }
    }
}
