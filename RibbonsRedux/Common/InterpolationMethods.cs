using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RibbonsRedux.Common
{
    /// <summary>
    /// Linearly interpolates between two values.
    /// </summary>
    public class LinearInterpolation : Interpolation
    {
        public override float Interpolate(float a, float b, float t)
        {
            return MathHelper.Lerp(a, b, t);
        }
    }

    /// <summary>
    /// Interpolates with a smooth step function over two values.
    /// </summary>
    public class SmoothStepInerpolation : Interpolation
    {
        public override float Interpolate(float a, float b, float t)
        {
            return MathHelper.SmoothStep(a, b, t);
        }
    }
}
