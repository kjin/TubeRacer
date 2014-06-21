using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RibbonsRedux.Graphics
{
    /// <summary>
    /// A mesh that can be drawn by Canvas.
    /// </summary>
    /// <typeparam name="VertexType">The vertex declaration type of the vertices of the mesh.</typeparam>
    /// <typeparam name="IndexType">The type of indices. This should be short or int.</typeparam>
    public abstract class UserIndexedPrimitives<VertexType, IndexType>
        where VertexType : struct, IVertexType
    {
        int ticks;
        public int Ticks { get { return ticks; } set { ticks = value; } }

        public UserIndexedPrimitives()
        {
            ticks = 0;
        }

        public void Tick()
        {
            ticks++;
        }

        public abstract VertexType[] Vertices { get; }
        public abstract IndexType[] Indices { get; }
        public abstract PrimitiveType PrimitiveType { get; }
    }
}
