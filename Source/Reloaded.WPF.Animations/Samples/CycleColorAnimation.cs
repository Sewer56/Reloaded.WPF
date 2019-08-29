using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Reloaded.WPF.Animations.Utilities;

namespace Reloaded.WPF.Animations.Samples
{
    /// <summary>
    /// An animation that Hue cycles a given colour.
    /// </summary>
    public class CycleColorAnimation : ManualAnimation<Color>
    {
        /// <summary>
        /// Constructs a <see cref="CycleColorAnimation"/> given a duration, framerate and base colour.
        /// </summary>
        public CycleColorAnimation(Action<Color> setColorFunction, Color baseColor, float duration, float frameRate) : base(duration, frameRate)
        {
            var lch = baseColor.ToLch();
            InterpolationMethod = time => ColorInterpolator.GetRainbowColor((float) lch.C, (float) lch.L, time).ToColor();
            ExecutionMethod     = setColorFunction;
        }
    }
}
