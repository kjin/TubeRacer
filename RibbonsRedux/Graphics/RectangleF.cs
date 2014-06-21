using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RibbonsRedux.Graphics
{
    public struct RectangleF
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;

        public RectangleF(float left, float right, float top, float bottom)
        {
            Left = Math.Min(left, right);
            Right = Math.Max(left, right);
            Top = Math.Min(top, bottom);
            Bottom = Math.Max(top, bottom);
        }
        public RectangleF(Rectangle rect) : this(rect.Left, rect.Right, rect.Top, rect.Bottom) { }

        public float Width { get { return Right - Left; } }
        public float Height { get { return Bottom - Top; } }
        public Vector2 Center { get { return new Vector2((Left + Right) / 2, (Top + Bottom) / 2); } }
        public Vector2 TopLeft { get { return new Vector2(Left, Top); } }
        public Vector2 TopRight { get { return new Vector2(Right, Top); } }
        public Vector2 BottomLeft { get { return new Vector2(Left, Bottom); } }
        public Vector2 BottomRight { get { return new Vector2(Right, Bottom); } }
        public RectangleF Inflate(float amountX, float amountY)
        {
            return new RectangleF(Left - amountX, Right + amountX, Top - amountY, Bottom + amountY);
        }
        public RectangleF Inflate(Vector2 amount) { return Inflate(amount.X, amount.Y); }
        public RectangleF Inflate(float amount) { return Inflate(amount, amount); }
        public RectangleF Envelope(Vector2 point)
        {
            return new RectangleF(Math.Min(Left, point.X), Math.Max(Right, point.X), Math.Min(Top, point.Y), Math.Max(Bottom, point.Y));
        }
        public RectangleF Envelope(RectangleF other)
        {
            return new RectangleF(Math.Min(Left, other.Left), Math.Max(Right, other.Right), Math.Min(Top, other.Top), Math.Max(Bottom, other.Bottom));
        }
        public Vector2 EdgeDistance(Vector2 point)
        {
            Vector2 ret = new Vector2();
            if (point.X > Right)
                ret.X = point.X - Right;
            else if (point.X < Left)
                ret.X = point.X - Left;
            if (point.Y > Bottom)
                ret.Y = point.Y - Bottom;
            else if (point.Y < Top)
                ret.Y = point.Y - Top;
            return ret;
        }
        public Vector2 ProjectPoint(Vector2 point)
        {
            point.X = Math.Min(Math.Max(point.X, Left), Right);
            point.Y = Math.Min(Math.Max(point.Y, Top), Bottom);
            return point;
        }
        public bool Contains(Vector2 point)
        {
            return point.X > Left && point.X < Right && point.Y > Top && point.Y < Bottom;
        }
        public RectangleF Offset(Vector2 amount)
        {
            return new RectangleF(Left + amount.X, Right + amount.X, Top + amount.Y, Bottom + amount.Y);
        }
        public override string ToString()
        {
            return String.Format("{{Left={0},Right={1},Top={2},Bottom={3}}}", Left, Right, Top, Bottom);
        }
    }
}
