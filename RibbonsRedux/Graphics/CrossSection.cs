using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RibbonsRedux.Graphics
{
    /// <summary>
    /// A class that specifies the shape of a path's cross-section.
    /// </summary>
    /// <typeparam name="VertexType">The vertex declaration type of the path mesh.</typeparam>
    public interface CrossSection<VertexType>
    {
        void GenerateCrossSection(VertexType[] vertices, int index, float t, Vector2 position, Vector2 normal);

        int LoopSize { get; }
    }

    /// <summary>
    /// A rectangular cross-section, used for drawing the ribbon and ground edge graphics.
    /// </summary>
    public class RectangleCrossSection : CrossSection<VertexPositionNormalTexture>
    {
        Vector2 diagonal;

        public RectangleCrossSection(Vector2 diagonal)
        {
            this.diagonal = diagonal;
        }

        public void GenerateCrossSection(VertexPositionNormalTexture[] vertices, int crossSection, float t, Vector2 position, Vector2 normal)
        {
            int n = crossSection * LoopSize;
            float angle = (float)Math.Atan2(normal.Y, normal.X);
            float cosa = (float)Math.Cos(angle);
            float sina = (float)Math.Sin(angle);
            float sinay = sina * diagonal.Y;
            float cosay = cosa * diagonal.Y;
            vertices[n + 0].Position = vertices[n + 1].Position = new Vector3(-sinay, cosay, diagonal.X);
            vertices[n + 2].Position = vertices[n + 3].Position = new Vector3(sinay, -cosay, diagonal.X);
            vertices[n + 4].Position = vertices[n + 5].Position = new Vector3(sinay, -cosay, -diagonal.X);
            vertices[n + 6].Position = vertices[n + 7].Position = new Vector3(-sinay, cosay, -diagonal.X);
            vertices[n + 7].Normal = vertices[n + 0].Normal = new Vector3(0,0, 1);
            vertices[n + 1].Normal = vertices[n + 2].Normal = new Vector3(0, 0, 1);
            vertices[n + 3].Normal = vertices[n + 4].Normal = new Vector3(0,0,-1);
            vertices[n + 5].Normal = vertices[n + 6].Normal = new Vector3(0, 0, -1);
            for (int i = 0; i < 8; i++)
                vertices[n + i].Position += new Vector3(position.X, position.Y, 0);
            vertices[n + 1].TextureCoordinate = new Vector2(0, t);
            vertices[n + 2].TextureCoordinate = vertices[n + 3].TextureCoordinate = new Vector2(0.25f, t);
            vertices[n + 4].TextureCoordinate = vertices[n + 5].TextureCoordinate = new Vector2(0.5f, t);
            vertices[n + 6].TextureCoordinate = vertices[n + 7].TextureCoordinate = new Vector2(0.75f, t);
            vertices[n + 0].TextureCoordinate = new Vector2(1, t);
        }

        public int LoopSize
        {
            get { return 8; }
        }
    }
}
