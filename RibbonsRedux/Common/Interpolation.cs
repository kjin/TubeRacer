using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RibbonsRedux.Common
{
    /// <summary>
    /// An abstract class that represents a method of interpolation between two values.
    /// </summary>
    public abstract class Interpolation
    {
        /// <summary>
        /// Interpolate between two float values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="t">The interpolation parameter, between 0 and 1.</param>
        /// <returns>The interpolated value.</returns>
        public abstract float Interpolate(float a, float b, float t);
        /// <summary>
        /// Interpolate between two Vector2 values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="t">The interpolation parameter, between 0 and 1.</param>
        /// <returns>The interpolated value.</returns>
        public Vector2 Interpolate(Vector2 a, Vector2 b, float t) { return new Vector2(Interpolate(a.X, b.X, t), Interpolate(a.Y, b.Y, t)); }
        /// <summary>
        /// Interpolate between two Vector3 values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="t">The interpolation parameter, between 0 and 1.</param>
        /// <returns>The interpolated value.</returns>
        public Vector3 Interpolate(Vector3 a, Vector3 b, float t) { return new Vector3(Interpolate(a.X, b.X, t), Interpolate(a.Y, b.Y, t), Interpolate(a.Z, b.Z, t)); }
        /// <summary>
        /// Interpolate between two Vector4 values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="t">The interpolation parameter, between 0 and 1.</param>
        /// <returns>The interpolated value.</returns>
        public Vector4 Interpolate(Vector4 a, Vector4 b, float t) { return new Vector4(Interpolate(a.X, b.X, t), Interpolate(a.Y, b.Y, t), Interpolate(a.Z, b.Z, t), Interpolate(a.W, b.W, t)); }
        /// <summary>
        /// Interpolate between two Color values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="t">The interpolation parameter, between 0 and 1.</param>
        /// <returns>The interpolated value.</returns>
        public Color Interpolate(Color a, Color b, float t) { return new Color(Interpolate(a.R, b.R, t), Interpolate(a.G, b.G, t), Interpolate(a.B, b.B, t), Interpolate(a.A, b.A, t)); }
    }
}
