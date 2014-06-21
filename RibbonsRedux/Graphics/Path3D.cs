using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace RibbonsRedux.Graphics
{
    /// <summary>
    /// A class that holds data for a path mesh, used for the ribbon and ground edge graphics.
    /// </summary>
    public class Path3D : UserIndexedPrimitives<VertexPositionNormalTexture, short>
    {
        VertexPositionNormalTexture[] vertices;
        short[] indices;
        int loops;

        float totalLength;

        float cornerRadius;
        int cornerResolution;

        public override VertexPositionNormalTexture[] Vertices { get { return vertices; } }
        public override short[] Indices { get { return indices; } }
        public override PrimitiveType PrimitiveType { get { return PrimitiveType.TriangleList; } }

        /// <summary>
        /// Constructs a new 3D path.
        /// </summary>
        /// <param name="controlPoints">The control points that define the path.</param>
        /// <param name="offset">For looped ribbons, controls the amount the path is offset from the control points.</param>
        /// <param name="cornerRadius">The radius of the rounded corners. Specify zero for no rounding.</param>
        /// <param name="cornerResolution">The resolution of the mesh at the corners. This should be at least 1, and exactly one for unrounded corners.</param>
        /// <param name="crossSection">The path's cross-section.</param>
        public Path3D(List<Vector2> controlPoints, float offset, float cornerRadius, int cornerResolution, CrossSection<VertexPositionNormalTexture> crossSection, bool loop)
        {
            if (cornerResolution <= 0) cornerResolution = 1;
            this.cornerRadius = cornerRadius;
            this.cornerResolution = cornerResolution;
            //filter corners
            List<Vector2> corners = new List<Vector2>();
            if (loop)
            {
                int distinctPoints = controlPoints.Count;
                if (controlPoints[0] == controlPoints[controlPoints.Count - 1])
                    distinctPoints--;
                for (int i = 0; i < distinctPoints; i++)
                {
                    Vector2 currToPrev = controlPoints[i] - controlPoints[(i + distinctPoints - 1) % distinctPoints];
                    Vector2 currToNext = controlPoints[i] - controlPoints[(i + 1) % distinctPoints];
                    if (GraphicsHelper.Cross(currToPrev, currToNext) == 0)
                        continue;
                    corners.Add(controlPoints[i] + offset * (currToPrev + currToNext));
                    loops = corners.Count * (cornerResolution + 1);
                }
            }
            else
            {
                
                corners.Add(controlPoints[0]);
                for (int i = 1; i < controlPoints.Count - 1; i++)
                {
                    Vector2 currToPrev = controlPoints[i] - controlPoints[i - 1];
                    Vector2 currToNext = controlPoints[i] - controlPoints[i + 1];
                    if (GraphicsHelper.Cross(currToPrev, currToNext) == 0)
                        continue;
                    corners.Add(controlPoints[i]);
                }
                corners.Add(controlPoints[controlPoints.Count - 1]);
                loops = (corners.Count - 2) * (cornerResolution + 1) + 2;
            }
            if (corners.Count == 0)
            {
                vertices = new VertexPositionNormalTexture[0];
                indices = new short[0];
                return;
            }

            int loopSize = crossSection.LoopSize;
            vertices = new VertexPositionNormalTexture[loopSize * loops];
            int loopNumber = 0;
            totalLength = 0;
            for (int i = 1; i < corners.Count; i++)
                totalLength += (corners[i - 1] - corners[i]).Length();
            if (loop)
                totalLength += (corners[0] - corners[corners.Count - 1]).Length();
            float localLength = 0;

            if (loop)
            {
                //define ranges
                for (int i = 0; i < corners.Count; i++)
                {
                    int iPrev = (i + corners.Count - 1) % corners.Count;
                    int iNext = (i + 1) % corners.Count;
                    if (i > 0)
                        localLength += (corners[iPrev] - corners[i]).Length();
                    GenerateCorner(corners[iPrev], corners[i], corners[iNext], crossSection, localLength, ref loopNumber);
                }
            }
            else
            {
                crossSection.GenerateCrossSection(vertices, loopNumber++, 0, corners[0], corners[1] - corners[0]);
                for (int i = 1; i < corners.Count - 1; i++)
                {
                    localLength += (corners[i - 1] - corners[i]).Length();
                    GenerateCorner(corners[i - 1], corners[i], corners[i + 1], crossSection, localLength, ref loopNumber);
                }
                crossSection.GenerateCrossSection(vertices, loopNumber, 1, corners[corners.Count - 1], corners[corners.Count - 1] - corners[corners.Count - 2]);
            }

            //indices
            int max = loops;
            if (loop)
                indices = new short[6 * loopSize * loops / 2];
            else
            {
                max = loops - 1;
                indices = new short[6 * loopSize * (loops - 1) / 2];
            }
            int index = 0;
            for (int i = 0; i < max; i++)
            {
                for (int j = 1; j < loopSize; j += 2)
                {
                    int j2 = (j + 1) % loopSize;
                    int i2 = (i + 1) % loops;
                    indices[index + 5] = indices[index + 0] = (short)(i * loopSize + j);
                    indices[index + 1] = (short)(i * loopSize + j2);
                    indices[index + 4] = (short)(i2 * loopSize + j);
                    indices[index + 3] = indices[index + 2] = (short)(i2 * loopSize + j2);
                    index += 6;
                }
            }
        }

        private void GenerateCorner(Vector2 prev, Vector2 curr, Vector2 next, CrossSection<VertexPositionNormalTexture> crossSection, float localLength, ref int loopNumber)
        {
            Vector2 currToPrev = curr - prev;
            Vector2 currToNext = curr - next;
            currToPrev.Normalize();
            currToNext.Normalize();
            float theta = MathHelper.Pi - (float)Math.Acos(Vector2.Dot(currToPrev, currToNext));
            float arcLength = cornerRadius * theta;
            Vector2 center = curr - (currToPrev + currToNext) * cornerRadius;
            float startAngle = (float)Math.Atan2(currToNext.Y, currToNext.X);
            float endAngle = (float)Math.Atan2(currToPrev.Y, currToPrev.X);
            if (startAngle - endAngle > MathHelper.Pi)
                endAngle += MathHelper.TwoPi;
            if (endAngle - startAngle > MathHelper.Pi)
                startAngle += MathHelper.TwoPi;
            for (int j = 0; j <= cornerResolution; j++)
            {
                float angle = MathHelper.Lerp(startAngle, endAngle, (float)j / cornerResolution);
                Vector2 v = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                float numer = localLength + arcLength * j / cornerResolution - arcLength / 2;
                while (numer < 0) numer += totalLength;
                crossSection.GenerateCrossSection(vertices, loopNumber++, numer / totalLength, center + cornerRadius * v, GraphicsHelper.Cross(currToPrev, currToNext) * new Vector2(v.Y, -v.X));
            }
        }

        /// <summary>
        /// The length of this path.
        /// </summary>
        public float TotalLength
        {
            get
            {
                return totalLength;
            }
        }

        protected void Dump(string file)
        {
            using (TextWriter tw = new StreamWriter(file))
            {
                for (int i = 0; i < vertices.Length; i++)
                    tw.WriteLine("{0} {1} {2}", vertices[i].Position.X, vertices[i].Position.Y, vertices[i].Position.Z);
            }
        }
    }
}
