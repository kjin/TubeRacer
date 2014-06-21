using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RibbonsRedux.Context
{
    public struct MultiOptionArrangement
    {
        public int Columns;
        public int Rows;
        public bool RowMajor;

        public MultiOptionArrangement(int columns, int rows, bool rowMajor = true)
        {
            Columns = columns;
            Rows = rows;
            RowMajor = rowMajor;
        }

        public Vector2 GetPosition(int index)
        {
            return RowMajor ? new Vector2(index % Columns, index / Columns) : new Vector2(index / Rows, index % Rows);
        }

        public int GetIndex(int x, int y)
        {
            return RowMajor ? y * Columns + x : x * Rows + y;
        }

        static MultiOptionArrangement listX = new MultiOptionArrangement(1, 0);
        public static MultiOptionArrangement ListX { get { return listX; } }
        static MultiOptionArrangement listY = new MultiOptionArrangement(0, 1);
        public static MultiOptionArrangement ListY { get { return listY; } }

        public static bool operator ==(MultiOptionArrangement a, MultiOptionArrangement b)
        {
            return a.Columns == b.Columns && a.Rows == b.Rows && a.RowMajor == b.RowMajor;
        }

        public static bool operator !=(MultiOptionArrangement a, MultiOptionArrangement b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return RowMajor ? ((Columns.GetHashCode() << 16) + (Rows.GetHashCode() >> 16)) : ~((Columns.GetHashCode() << 16) + (Rows.GetHashCode() >> 16));
        }
    }
}
