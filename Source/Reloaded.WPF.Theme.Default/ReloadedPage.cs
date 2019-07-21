#pragma warning disable 1591
using Reloaded.WPF.Pages;
using Reloaded.WPF.Pages.Animations;
using Reloaded.WPF.Pages.Animations.Enum;
using Reloaded.WPF.Utilities;

namespace Reloaded.WPF.Theme.Default
{
    public class ReloadedPage : PageBase
    {
        private XamlResource<double> _xamlEntrySlideAnimationDuration;
        private XamlResource<double> _xamlEntryFadeAnimationDuration;
        private XamlResource<double> _xamlEntryFadeOpacityStart;

        private XamlResource<double> _xamlExitSlideAnimationDuration;
        private XamlResource<double> _xamlExitFadeAnimationDuration;
        private XamlResource<double> _xamlExitFadeOpacityEnd;

        public ReloadedPage()
        {
            // We play the animation once the content is rendered.
            // So before we play the animation, we must hide it so the first frame is not seen.

            this.Loaded += (sender, args) => Loader.Load(this);
            var thisArray = new[] { this };

            _xamlEntrySlideAnimationDuration    = new XamlResource<double>("EntrySlideAnimationDuration", thisArray, this);
            _xamlEntryFadeAnimationDuration     = new XamlResource<double>("EntryFadeAnimationDuration", thisArray, this);
            _xamlEntryFadeOpacityStart          = new XamlResource<double>("EntryFadeOpacityStart", thisArray, this);

            _xamlExitSlideAnimationDuration     = new XamlResource<double>("ExitSlideAnimationDuration", thisArray, this);
            _xamlExitFadeAnimationDuration      = new XamlResource<double>("ExitFadeAnimationDuration", thisArray, this);
            _xamlExitFadeOpacityEnd             = new XamlResource<double>("ExitFadeOpacityEnd", thisArray, this);
        }

        /// <summary>
        /// Creates instances of the animations that are ran on entering the page.
        /// Note: Override this to modify animations used by page.
        /// </summary>
        protected override Animation[] MakeEntryAnimations()
        {
            return new Animation[]
            {
                new RenderTransformAnimation(-this.ActualWidth, RenderTransformDirection.Horizontal, RenderTransformTarget.Towards, null, _xamlEntrySlideAnimationDuration.Get()),
                new OpacityAnimation(_xamlEntryFadeAnimationDuration.Get(), _xamlEntryFadeOpacityStart.Get(), 1)
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
                new RenderTransformAnimation(this.ActualWidth, RenderTransformDirection.Horizontal, RenderTransformTarget.Away, null, _xamlExitSlideAnimationDuration.Get()),
                new OpacityAnimation(_xamlExitFadeAnimationDuration.Get(), 1, _xamlExitFadeOpacityEnd.Get())
            };
        }
    }
}
