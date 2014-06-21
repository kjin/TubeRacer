using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RibbonsRedux.Graphics;

namespace TubeLibrary
{
    public class AxisAlignedRectangle : UserIndexedPrimitives<VertexPositionNormalTexture, short>
    {
        VertexPositionNormalTexture[] vertices;
        short[] indices;

        public AxisAlignedRectangle(Vector3 center, Vector3 size)
        {
            TriangleMeshBuilder tmb = new TriangleMeshBuilder(0, 6);
            tmb.BuildAxisAlignedBox(center - size / 2, center + size / 2);
            vertices = new VertexPositionNormalTexture[tmb.Vertices.Length];
            indices = new short[tmb.Indices.Length];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = tmb.Vertices[i].ToVertexPositionNormalTexture(Vector2.Zero);
            for (int i = 0; i < indices.Length; i++)
                indices[i] = (short)tmb.Indices[i];
        }

        public override VertexPositionNormalTexture[] Vertices { get { return vertices; } }

        public override short[] Indices { get { return indices; } }

        public override PrimitiveType PrimitiveType { get { return PrimitiveType.TriangleList; } }
    }
}
