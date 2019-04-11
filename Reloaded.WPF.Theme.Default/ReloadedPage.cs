using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Reloaded.WPF.Pages;
using Reloaded.WPF.Pages.Animations;
using Reloaded.WPF.Pages.Animations.Enum;
using Reloaded.WPF.Utilities;

namespace Reloaded.WPF.Theme.Default
{
    public class ReloadedPage : PageBase
    {
        #region XAML_RESOURCE NAMES
        public const string XAML_ENTRYSLIDEANIMATIONDURATION = "EntrySlideAnimationDuration";
        public const string XAML_ENTRYFADEANIMATIONDURATION  = "EntryFadeAnimationDuration";
        public const string XAML_ENTRYFADEOPACITYSTART       = "EntryFadeOpacityStart";

        public const string XAML_EXITSLIDEANIMATIONDURATION = "ExitSlideAnimationDuration";
        public const string XAML_EXITFADEANIMATIONDURATION  = "ExitFadeAnimationDuration";
        public const string XAML_EXITFADEOPACITYEND         = "ExitFadeOpacityEnd";
        #endregion

        private ResourceManipulator ResourceManipulator { get; }

        #region Animations

        public ReloadedPage()
        {
            // We play the animation once the content is rendered.
            // So before we play the animation, we must hide it so the first frame is not seen.

            this.Visibility = Visibility.Hidden;
            this.Loaded += (sender, args) => Loader.Load(this);
            ResourceManipulator = new ResourceManipulator(this);
        }

        /// <summary>
        /// Creates instances of the animations that are ran on entering the page.
        /// Note: Override this to modify animations used by page.
        /// </summary>
        protected Animation[] MakeEntryAnimations()
        {
            return new Animation[]
            {
                new RenderTransformAnimation(-this.ActualWidth, RenderTransformDirection.Horizontal, RenderTransformTarget.Towards, null, ResourceManipulator.Get<double>(XAML_ENTRYSLIDEANIMATIONDURATION)),
                new OpacityAnimation(ResourceManipulator.Get<double>(XAML_ENTRYFADEANIMATIONDURATION), ResourceManipulator.Get<double>(XAML_ENTRYFADEOPACITYSTART), 1)
            };
        }

        /// <summary>
        /// Creates instances of the animations that are ran on exiting the page.
        /// Note: Override this to modify animations used by page.
        /// </summary>
        protected Animation[] MakeExitAnimations()
        {
            return new Animation[]
            {
                new RenderTransformAnimation(this.ActualWidth, RenderTransformDirection.Horizontal, RenderTransformTarget.Away, null, ResourceManipulator.Get<double>(XAML_EXITSLIDEANIMATIONDURATION)),
                new OpacityAnimation(ResourceManipulator.Get<double>(XAML_EXITFADEANIMATIONDURATION), 1, ResourceManipulator.Get<double>(XAML_EXITFADEOPACITYEND))
            };
        }

        public override async Task AnimateIn()
        {
            this.Visibility = Visibility.Visible;

            /* Create Storyboard consisting of all animations. */
            var storyBoard = new Storyboard();
            var animations = MakeEntryAnimations();
            Animation.AddAnimations(storyBoard, animations, this);
            storyBoard.Begin(this);

            await Task.Delay(TimeSpan.FromSeconds(GetLongestAnimationDuration(storyBoard)));
        }

        public override async Task AnimateOut()
        {
            var storyBoard = new Storyboard();
            var animations = MakeExitAnimations();
            Animation.AddAnimations(storyBoard, animations, this);
            storyBoard.Begin(this);

            await Task.Delay(TimeSpan.FromSeconds(GetLongestAnimationDuration(storyBoard)));
        }

        #endregion
    }
}
