using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RibbonsRedux.Graphics
{
    public static partial class GraphicsHelper
    {
        static Random random = new Random();
        public static Random Random { get { return random; } }

        public static float FastStep(float t)
        {
            return MathHelper.SmoothStep(0, 1, t);
        }

        public static Rectangle[] SplitRectangle(Rectangle rect)
        {
            Rectangle[] rects = new Rectangle[rect.Width * rect.Height];
            int index = 0;
            for (int i = rect.Left; i < rect.Right; i++)
                for (int j = rect.Top; j < rect.Bottom; j++)
                    rects[index++] = new Rectangle(i, j, 1, 1);
            return rects;
        }

        /// <summary>
        /// Returns x as a valid index for an array of size m.
        /// </summary>
        /// <param name="x">The index to process.</param>
        /// <param name="m">The size of the array.</param>
        /// <returns>A valid index for the array.</returns>
        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        public static float Mod(float x, float m)
        {
            float r = x % m;
            return r < 0 ? r + m : r;
        }

        /// <summary>
        /// Compute the origin of a box based on its dimensions, and an "anchor" point.
        /// </summary>
        /// <param name="anchor">The location of the anchor point.</param>
        /// <param name="dimensions">The dimensions of the box.</param>
        /// <returns>The computed origin.</returns>
        public static Vector2 ComputeAnchorOrigin(Anchor anchor, Vector2 dimensions)
        {
            return new Vector2((int)anchor % 3 * dimensions.X / 2, (int)anchor / 3 * dimensions.Y / 2);
        }

        public static Anchor Reverse(Anchor anchor)
        {
            return (Anchor)(8 - (int)anchor);
        }

        public static Rectangle Scale(Rectangle rect, float factor)
        {
            return new Rectangle((int)(factor * rect.X), (int)(factor * rect.Y), (int)(factor * rect.Width), (int)(factor * rect.Height));
        }

        /// <summary>
        /// Apply a 4x4 matrix to a vector.
        /// </summary>
        /// <param name="m">The matrix to apply.</param>
        /// <param name="v">The vector to transform.</param>
        /// <returns>The transformed vector.</returns>
        public static Vector2 ApplyMatrix(Matrix m, Vector2 v)
        {
            Vector3 v3 = Vector3.Transform(new Vector3(v, 0), m);
            return new Vector2(v3.X, v3.Y);
        }

        /// <summary>
        /// Compute the angle for vectors where exactly one component is zero quickly, or returns zero when this is not the case.
        /// </summary>
        /// <param name="v">The vector input.</param>
        /// <returns>The value of Atan2(v.Y, v.X), in the range (-pi,pi].</returns>
        public static float FastOrthoAtan(Vector2 v)
        {
            if (v.X == 0)
                if (v.Y > 0)
                    return MathHelper.PiOver2;
                else if (v.Y < 0)
                    return -MathHelper.PiOver2;
            if (v.Y == 0)
                if (v.X > 0)
                    return 0;
                else if (v.X < 0)
                    return MathHelper.Pi;
            return 0;
        }

        /*
         *    |Angle|Tile
         * ----------------
         * L/U|   90|Corner
         * L/R|    0|Edge
         * L/D|  180|Corner
         * U/R|    0|Corner
         * U/D|   90|Edge
         * R/D/  270|Corner
         */

        /// <summary>
        /// Takes a specified point one unit closer to a destination. X is favored over Y.
        /// </summary>
        /// <param name="current">The point's current location.</param>
        /// <param name="destination">The point's desired location.</param>
        /// <returns></returns>
        public static Point Travel(Point current, Point destination)
        {
            if (current.X < destination.X)
                return new Point(current.X + 1, current.Y);
            else if (current.X > destination.X)
                return new Point(current.X - 1, current.Y);
            else if (current.Y < destination.Y)
                return new Point(current.X, current.Y - 1);
            else if (current.Y > destination.Y)
                return new Point(current.X, current.X + 1);
            else
                return current;
        }

        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static float ToRadians(RightAngleRotations r)
        {
            return MathHelper.PiOver2 * (int)r;
        }

        public static RightAngleRotations Add(RightAngleRotations r1, RightAngleRotations r2)
        {
            return (RightAngleRotations)(((int)r1 + (int)r2) % 4);
        }

        public static EdgeTypes Reverse(EdgeTypes edgeType)
        {
            if (edgeType == EdgeTypes.Left)
                return EdgeTypes.Right;
            else if (edgeType == EdgeTypes.Right)
                return EdgeTypes.Left;
            else if (edgeType == EdgeTypes.Up)
                return EdgeTypes.Down;
            else if (edgeType == EdgeTypes.Down)
                return EdgeTypes.Up;
            return EdgeTypes.None;
        }

        public static bool[,] GetFillMap(List<Rectangle> rectangles, out Rectangle boundingBox, bool includeLastRowColumn = false)
        {
            boundingBox = new Rectangle();
            if (rectangles.Count > 0) boundingBox = rectangles[0];
            for (int i = 1; i < rectangles.Count; i++)
                boundingBox = Rectangle.Union(boundingBox, rectangles[i]);
            bool[,] fillMap = new bool[boundingBox.Width + (includeLastRowColumn ? 1 : 0), boundingBox.Height + (includeLastRowColumn ? 1 : 0)];
            Point offset = new Point(-boundingBox.X, -boundingBox.Y);
            for (int i = 0; i < rectangles.Count; i++)
            {
                int xMax = rectangles[i].Right + (includeLastRowColumn ? 1 : 0);
                int yMax = rectangles[i].Bottom + (includeLastRowColumn ? 1 : 0);
                for (int x = rectangles[i].Left; x < xMax; x++)
                    for (int y = rectangles[i].Top; y < yMax; y++)
                        fillMap[x + offset.X, y + offset.Y] = true;
            }
            return fillMap;
        }

        public static List<List<Vector2>> GetBorders(List<Rectangle> rectangles)
        {
            Rectangle boundingBox = new Rectangle();
            bool[,] fillMap = GetFillMap(rectangles, out boundingBox);
            Point offset = new Point(-boundingBox.X, -boundingBox.Y);
            int[,] edgeMap = new int[boundingBox.Width + 1, boundingBox.Height + 1];
            for (int x = 0; x <= boundingBox.Width; x++)
                for (int y = 0; y <= boundingBox.Height; y++)
                {
                    bool topLeft = false, topRight = false, bottomLeft = false, bottomRight = false;
                    if (x > 0 && y > 0) topLeft = fillMap[x - 1, y - 1];
                    if (x > 0 && y < boundingBox.Height) bottomLeft = fillMap[x - 1, y];
                    if (x < boundingBox.Width && y > 0) topRight = fillMap[x, y - 1];
                    if (x < boundingBox.Width && y < boundingBox.Height) bottomRight = fillMap[x, y];
                    int flag = 0x0;
                    if (topLeft ^ bottomLeft) flag |= (int)EdgeTypes.Left;
                    if (bottomLeft ^ bottomRight) flag |= (int)EdgeTypes.Down;
                    if (topLeft ^ topRight) flag |= (int)EdgeTypes.Up;
                    if (topRight ^ bottomRight) flag |= (int)EdgeTypes.Right;
                    //if we somehow get 0xF, then either topleft/bottomright are filled, or topright/bottomleft.
                    //the 5th/6th bit is a "parity" bit.
                    if (flag == 0xF)
                        if (topLeft)
                            flag = 0x1F;
                        else
                            flag = 0x2F;
                    edgeMap[x, y] = flag;
                }
            //walk the line
            List<List<Vector2>> lists = new List<List<Vector2>>();
            for (int x = 0; x <= boundingBox.Width; x++)
                for (int y = 0; y <= boundingBox.Height; y++)
                {
                    if (edgeMap[x, y] == 0) continue;
                    List<Vector2> controlPoints = new List<Vector2>();
                    int x2 = x;
                    int y2 = y;
                    EdgeTypes nextDirection = EdgeTypes.None;
                    while (edgeMap[x, y] != 0)
                    {
                        controlPoints.Add(new Vector2(x2 - offset.X, y2 - offset.Y));
                        if ((edgeMap[x2, y2] & ~0xF) == 0 || nextDirection == EdgeTypes.None)
                        {
                            nextDirection = EdgeTypes.None;
                            for (int i = 0x1; i < 0x10; i <<= 1)
                                if ((edgeMap[x2, y2] & i) != 0)
                                    nextDirection = (EdgeTypes)i;
                        }
                        else
                        {
                            if ((edgeMap[x2, y2] & 0x10) != 0) //topLeft parity
                            {
                                if (nextDirection == EdgeTypes.Left)
                                    nextDirection = EdgeTypes.Up;
                                else if (nextDirection == EdgeTypes.Up)
                                    nextDirection = EdgeTypes.Left;
                                else if (nextDirection == EdgeTypes.Right)
                                    nextDirection = EdgeTypes.Down;
                                else if (nextDirection == EdgeTypes.Down)
                                    nextDirection = EdgeTypes.Right;
                                edgeMap[x2, y2] &= ~0x10;
                            }
                            else
                            {
                                if (nextDirection == EdgeTypes.Left)
                                    nextDirection = EdgeTypes.Down;
                                else if (nextDirection == EdgeTypes.Up)
                                    nextDirection = EdgeTypes.Right;
                                else if (nextDirection == EdgeTypes.Right)
                                    nextDirection = EdgeTypes.Up;
                                else if (nextDirection == EdgeTypes.Down)
                                    nextDirection = EdgeTypes.Left;
                                edgeMap[x2, y2] &= ~0x20;
                            }
                        }
                        edgeMap[x2, y2] &= ~(int)nextDirection;
                        if (nextDirection == EdgeTypes.Left)
                            x2 = x2 - 1;
                        else if (nextDirection == EdgeTypes.Right)
                            x2 = x2 + 1;
                        else if (nextDirection == EdgeTypes.Up)
                            y2 = y2 - 1;
                        else if (nextDirection == EdgeTypes.Down)
                            y2 = y2 + 1;
                        edgeMap[x2, y2] &= ~(int)Reverse(nextDirection);
                    }
                    lists.Add(controlPoints);
                }
            return lists;
        }
    }
}
