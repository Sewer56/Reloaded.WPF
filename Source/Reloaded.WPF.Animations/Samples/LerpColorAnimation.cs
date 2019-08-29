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
    /// An animation that fades a colour from colour A to colour B.
    /// </summary>
    public class LerpColorAnimation : ManualAnimation<Color>
    {
        /// <summary>
        /// Constructs a <see cref="CycleColorAnimation"/> given a duration, framerate and base colour.
        /// </summary>
        public LerpColorAnimation(Action<Color> setColorFunction, Color colorA, Color colorB, float duration, float frameRate) : base(duration, frameRate)
        {
            var lchSourceColor = colorA.ToLch();
            var lchTargetColor = colorB.ToLch();

            InterpolationMethod = time => ColorInterpolator.CalculateIntermediateColour(lchSourceColor, lchTargetColor, time).ToColor();
            ExecutionMethod     = setColorFunction;
        }
    }
}
