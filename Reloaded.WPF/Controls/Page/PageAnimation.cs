using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reloaded.WPF.Controls.Page
{
    public enum PageAnimation
    {
        /// <summary>
        /// No animation.
        /// </summary>
        None,

        /// <summary>
        /// Slides in from the left and fades in.
        /// </summary>
        SlideInFromLeftAndFade,

        /// <summary>
        /// Slides out to the right and fades in.
        /// </summary>
        SlideOutToRightAndFade
    }
}
