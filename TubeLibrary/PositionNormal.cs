using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TubeLibrary
{
    public struct PositionNormal
    {
        public Vector3 Position;
        public Vector3 Normal;

        public PositionNormal(Vector3 position)
        {
            Position = position;
            Normal = Vector3.Zero;
        }

        public PositionNormal(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }

        public VertexPositionColor ToVertexPositionColor(Color color)
        {
            return new VertexPositionColor(Position, color);
        }

        public VertexPositionColorTexture ToVertexPositionColorTexture(Color color, Vector2 texCoords)
        {
            return new VertexPositionColorTexture(Position, color, texCoords);
        }

        public VertexPositionNormalTexture ToVertexPositionNormalTexture(Vector2 texCoords)
        {
            return new VertexPositionNormalTexture(Position, Normal, texCoords);
        }

        public VertexPositionTexture ToVertexPositionTexture(Vector2 texCoords)
        {
            return new VertexPositionTexture(Position, texCoords);
        }

        public VertexPositionNormalColor ToVertexPositionNormalColor(Color color)
        {
            return new VertexPositionNormalColor(Position, Normal, color);
        }
    }
}
