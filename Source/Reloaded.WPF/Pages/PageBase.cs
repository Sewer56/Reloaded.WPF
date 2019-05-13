using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Reloaded.WPF.Pages.Animations;

namespace Reloaded.WPF.Pages
{
    /// <summary>
    /// Abstracts an individual <see cref="System.Windows.Controls.Page"/> of a window.
    /// </summary>
    public abstract class PageBase : System.Windows.Controls.Page
    {
        /// <summary>
        /// Called after the page finished animating in.
        /// </summary>
        public event Action AnimateInFinished = () => { };

        /// <summary>
        /// Called after the page finished animating out.
        /// </summary>
        public event Action AnimateOutFinished = () => { };

        /// <summary/>
        protected PageBase()
        {
            this.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Creates instances of the animations that are ran on entering the page.
        /// Note: Override this to modify animations used by page.
        /// </summary>
        protected abstract Animation[] MakeEntryAnimations();

        /// <summary>
        /// Creates instances of the animations that are ran on exiting the page.
        /// Note: Override this to modify animations used by page.
        /// </summary>
        protected abstract Animation[] MakeExitAnimations();

        /// <summary>
        /// Plays the entry animation for this page.
        /// </summary>
        public virtual async Task AnimateIn()
        {
            this.Visibility = Visibility.Visible;

            /* Create Storyboard consisting of all animations. */
            var storyBoard = new Storyboard();
            var animations = MakeEntryAnimations();
            Animation.AddAnimations(storyBoard, animations, this);
            storyBoard.Begin(this);

            await Task.Delay(TimeSpan.FromSeconds(GetLongestAnimationDuration(storyBoard)));
            AnimateInFinished();
        }

        /// <summary>
        /// Plays the exit animation for this page.
        /// </summary>
        public virtual async Task AnimateOut()
        {
            var storyBoard = new Storyboard();
            var animations = MakeExitAnimations();
            Animation.AddAnimations(storyBoard, animations, this);
            storyBoard.Begin(this);

            await Task.Delay(TimeSpan.FromSeconds(GetLongestAnimationDuration(storyBoard)));
            AnimateOutFinished();
        }

        /// <summary>
        /// Retrieves the longest animation assigned to a storyboard in seconds.
        /// </summary>
        protected double GetLongestAnimationDuration(Storyboard storyBoard)
        {
            return storyBoard.Children.Max(x => x.Duration.TimeSpan.TotalSeconds);
        }
    }
}
