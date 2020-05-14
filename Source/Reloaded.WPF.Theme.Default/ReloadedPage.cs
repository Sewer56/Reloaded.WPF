#pragma warning disable 1591
using Reloaded.WPF.Pages;
using Reloaded.WPF.Pages.Animations;
using Reloaded.WPF.Pages.Animations.Enum;
using Reloaded.WPF.Utilities;

namespace Reloaded.WPF.Theme.Default
{
    public class ReloadedPage : PageBase
    {
        protected XamlResource<double> XamlEntrySlideAnimationDuration;
        protected XamlResource<double> XamlEntryFadeAnimationDuration;
        protected XamlResource<double> XamlEntryFadeOpacityStart;

        protected XamlResource<double> XamlExitSlideAnimationDuration;
        protected XamlResource<double> XamlExitFadeAnimationDuration;
        protected XamlResource<double> XamlExitFadeOpacityEnd;

        public ReloadedPage()
        {
            // We play the animation once the content is rendered.
            // So before we play the animation, we must hide it so the first frame is not seen.
            XamlEntrySlideAnimationDuration    = new XamlResource<double>("EntrySlideAnimationDuration");
            XamlEntryFadeAnimationDuration     = new XamlResource<double>("EntryFadeAnimationDuration");
            XamlEntryFadeOpacityStart          = new XamlResource<double>("EntryFadeOpacityStart");

            XamlExitSlideAnimationDuration     = new XamlResource<double>("ExitSlideAnimationDuration");
            XamlExitFadeAnimationDuration      = new XamlResource<double>("ExitFadeAnimationDuration");
            XamlExitFadeOpacityEnd             = new XamlResource<double>("ExitFadeOpacityEnd");
        }

        /// <summary>
        /// Creates instances of the animations that are ran on entering the page.
        /// Note: Override this to modify animations used by page.
        /// </summary>
        protected override Animation[] MakeEntryAnimations()
        {
            return new Animation[]
            {
                new RenderTransformAnimation(-this.ActualWidth, RenderTransformDirection.Horizontal, RenderTransformTarget.Towards, null, XamlEntrySlideAnimationDuration.Get()),
                new OpacityAnimation(XamlEntryFadeAnimationDuration.Get(), XamlEntryFadeOpacityStart.Get(), 1)
            };
        }

        /// <summary>
        /// Creates instances of the animations that are ran on exiting the page.
        /// Note: Override this to modify animations used by page.
        /// </summary>
        protected override Animation[] MakeExitAnimations()
        {
            return new Animation[]
            {
                new RenderTransformAnimation(this.ActualWidth, RenderTransformDirection.Horizontal, RenderTransformTarget.Away, null, XamlExitSlideAnimationDuration.Get()),
                new OpacityAnimation(XamlExitFadeAnimationDuration.Get(), 1, XamlExitFadeOpacityEnd.Get())
            };
        }
    }
}
