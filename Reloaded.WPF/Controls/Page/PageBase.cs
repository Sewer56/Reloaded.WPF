using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Reloaded.WPF.Controls.Page
{
    /// <summary>
    /// Abstracts an individual <see cref="System.Windows.Controls.Page"/> of a window.
    /// </summary>
    public class PageBase : System.Windows.Controls.Page
    {
        /// <summary> Animation played when the page enters the screen. </summary>
        public PageAnimation EntryAnimation { get; set; } = PageAnimation.SlideInFromLeftAndFade;

        /// <summary> Animation played when the page leaves the screen. </summary>
        public PageAnimation ExitAnimation { get; set; } = PageAnimation.SlideOutToRightAndFade;

        /// <summary> The amount of time in seconds the page entry animation plays for. </summary>
        public float EntryAnimationLength = 0.842F;

        /// <summary> The amount of time in seconds the page exit animation plays for. </summary>
        public float ExitAnimationLength = 0.842F;

        public PageBase()
        {
            /* The page may flicker in when first loaded if not set to invisible before animation. */
            if (this.EntryAnimation != PageAnimation.SlideInFromLeftAndFade)
                this.Visibility = System.Windows.Visibility.Collapsed;

            /* Play animation on load. */
            this.Loaded += PageBase_Loaded;
        }

        /// <summary>
        /// Executed when the page is loaded.
        /// </summary>
        private async void PageBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await AnimateIn();
        }

        /// <summary>
        /// Plays the entry animation for this page.
        /// </summary>
        public async Task AnimateIn()
        {
            switch (EntryAnimation)
            {
                case PageAnimation.None:
                    break;

                case PageAnimation.SlideInFromLeftAndFade:

                    var storyBoard = new Storyboard();
                    var slideAnimation = new ThicknessAnimation();
                    slideAnimation.Duration = new Duration(TimeSpan.FromSeconds(this.EntryAnimationLength));
                    slideAnimation.From = new Thickness(- this.WindowWidth, 0, this.WindowWidth, 0);
                    slideAnimation.To = new Thickness(0);
                    slideAnimation.DecelerationRatio = 0.9F;

                    Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("Margin"));
                    storyBoard.Children.Add(slideAnimation);
                    storyBoard.Begin(this);

                    this.Visibility = Visibility.Visible;

                    await Task.Delay(TimeSpan.FromSeconds((double) this.EntryAnimation));

                    break;

                case PageAnimation.SlideOutToRightAndFade:

                    break;
            }
        }



        /// <summary>
        /// Plays the exit animation for this page.
        /// </summary>
        public async Task AnimateOut()
        {
            switch (ExitAnimation)
            {
                case PageAnimation.None:
                    break;

                case PageAnimation.SlideInFromLeftAndFade:

                    break;

                case PageAnimation.SlideOutToRightAndFade:

                    var storyBoard = new Storyboard();
                    var slideAnimation = new ThicknessAnimation();
                    slideAnimation.Duration = new Duration(TimeSpan.FromSeconds(this.EntryAnimationLength));
                    slideAnimation.From = new Thickness(0, 0, 0, 0);
                    slideAnimation.To = new Thickness(this.WindowWidth, 0, -this.WindowWidth, 0);
                    slideAnimation.DecelerationRatio = 0.9F;

                    Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("Margin"));
                    storyBoard.Children.Add(slideAnimation);
                    storyBoard.Begin(this);

                    this.Visibility = Visibility.Visible;

                    await Task.Delay(TimeSpan.FromSeconds((double)this.EntryAnimation));

                    break;
            }
        }
    }
}
