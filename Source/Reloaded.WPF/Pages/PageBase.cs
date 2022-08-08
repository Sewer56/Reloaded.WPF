using System;
using System.Windows;
using System.Windows.Media.Animation;
using Reloaded.WPF.Controls;
using Reloaded.WPF.Pages.Animations;

namespace Reloaded.WPF.Pages
{
    /// <summary>
    /// Abstracts an individual <see cref="System.Windows.Controls.Page"/> of a window.
    /// </summary>
    public abstract class PageBase : System.Windows.Controls.Page
    {
        /// <summary>
        /// Called when the <see cref="PageHost"/> is about to remove the object from front view.
        /// </summary>
        public event Action SwappedOut = () => { };

        /// <summary>
        /// Called after the page started animating in.
        /// </summary>
        public event Action AnimateInStarted = () => { };

        /// <summary>
        /// Called after the page started animating out.
        /// </summary>
        public event Action AnimateOutStarted = () => { };

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
        public virtual void AnimateIn()
        {
            this.Visibility = Visibility.Visible;
            AnimateInStarted();

            /* Create Storyboard consisting of all animations. */
            var storyBoard = new Storyboard();
            var animations = MakeEntryAnimations();
            Animation.AddAnimations(storyBoard, animations, this);

            storyBoard.Completed += (sender, args) =>
            {
                AnimateInFinished();
            };

            storyBoard.Begin(this);
        }

        /// <summary>
        /// Plays the exit animation for this page.
        /// </summary>
        public virtual void AnimateOut()
        {
            AnimateOutStarted();

            var storyBoard = new Storyboard();
            var animations = MakeExitAnimations();
            Animation.AddAnimations(storyBoard, animations, this);

            storyBoard.Completed += (sender, args) =>
            {
                AnimateOutFinished();
            };

            storyBoard.Begin(this);
        }

        /// <summary>
        /// Calls the swapped out event.
        /// </summary>
        internal void InvokeSwappedOut() => SwappedOut();
    }
}
