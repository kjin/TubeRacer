using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RibbonsRedux.Graphics
{
    /// <summary>
    /// An enumeration of coordinate modes.
    /// </summary>
    public enum CoordinateMode
    {
        /// <summary>
        /// A coordinate system in physical coordinates, where the origin is the center of the object (used in main gameplay). This mode takes camera variables into account.
        /// </summary>
        PhysicalCoordinates,
        /// <summary>
        /// A coordinate system in screen coordinates, where the origin is in the top-left corner (used in the HUD and non-gameplay screens). This mode ignores the camera.
        /// </summary>
        ScreenCoordinates
    }

    /// <summary>
    /// An enumeration of right-angle rotations.
    /// </summary>
    public enum RightAngleRotations
    {
        Zero, Ninety, OneEighty, TwoSeventy
    }

    /// <summary>
    /// An enumeration of tileset components.
    /// </summary>
    public enum TilesetComponents
    {
        InsideCorner, Edge, OutsideCorner
    }

    /// <summary>
    /// An enumeration of anchor points for drawing sprites and text.
    /// </summary>
    public enum Anchor
    {
        TopLeft, TopCenter, TopRight, CenterLeft, Center, CenterRight, BottomLeft, BottomCenter, BottomRight
    }

    /// <summary>
    /// An enumeration of camera projection matrix options.
    /// </summary>
    public enum Projections
    {
        Behind = -1, Full = 0, Front = 1
    }

    public enum EdgeTypes
    {
        None = 0x0,
        Right = 0x1,
        Up = 0x2,
        Down = 0x4,
        Left = 0x8
    }
}
