using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Reloaded.WPF.Pages
{
    /// <summary>
    /// Abstracts an individual <see cref="System.Windows.Controls.Page"/> of a window.
    /// </summary>
    public abstract class PageBase : System.Windows.Controls.Page
    {
        /// <summary>
        /// Plays the entry animation for this page.
        /// </summary>
        public abstract Task AnimateIn();

        /// <summary>
        /// Plays the exit animation for this page.
        /// </summary>
        public abstract Task AnimateOut();

        /// <summary>
        /// Retrieves the longest animation assigned to a storyboard in seconds.
        /// </summary>
        protected double GetLongestAnimationDuration(Storyboard storyBoard)
        {
            return storyBoard.Children.Max(x => x.Duration.TimeSpan.TotalSeconds);
        }
    }
}
