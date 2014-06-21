using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RibbonsRedux.Graphics
{
    /// <summary>
    /// A class containing some vital constants for the graphics engine.
    /// </summary>
    public class GraphicsConstants
    {
        //these should largely be left untouched
        static float defaultPixelsPerUnit = 64.0f;
        static float defaultSpriteScale = 0.5f;
        static float defaultGraphicsScale = 1;
        //change these depending on what the startup res should be
        static int defaultWidth = 1280;
        static int defaultHeight = 720;
        static bool defaultFullScreen = false;

        public static float DEFAULT_PIXELS_PER_UNIT { get { return defaultPixelsPerUnit; } }
        public static Vector2 DEFAULT_DIMENSIONS { get { return new Vector2(defaultWidth, defaultHeight); } }

        static float pixelsPerUnit = defaultPixelsPerUnit;
        /// <summary>
        /// The size of a physical unit in pixels.
        /// </summary>
        public static float PIXELS_PER_UNIT { get { return pixelsPerUnit; } }

        static float graphicsScale = defaultGraphicsScale;
        /// <summary>
        /// The global sprite drawing scale. For example, if all textures are imported at double size, we want scale to be 0.5.
        /// </summary>
        public static float SPRITE_SCALE { get { return defaultSpriteScale; } }
        /// <summary>
        /// The global background drawing scale.
        /// </summary>
        public static float GRAPHICS_SCALE { get { return graphicsScale; } }

        static int viewportWidth = defaultWidth;
        static int viewportHeight = defaultHeight;
        public static Vector2 VIEWPORT_DIMENSIONS { get { return new Vector2(viewportWidth, viewportHeight); } }
        /// <summary>
        /// The default width of the camera/gameplay window.
        /// </summary>
        public static int VIEWPORT_WIDTH
        {
            get { return viewportWidth; }
            set
            {
                viewportWidth = value;
                float multiplier = Math.Max((float)viewportWidth / defaultWidth, (float)viewportHeight / defaultHeight);
                pixelsPerUnit = defaultPixelsPerUnit * multiplier;
                graphicsScale = defaultGraphicsScale * multiplier;
            }
        }
        /// <summary>
        /// The default height of the camera/gameplay window.
        /// </summary>
        public static int VIEWPORT_HEIGHT
        {
            get { return viewportHeight; }
            set
            {
                viewportHeight = value;
                float multiplier = Math.Max((float)viewportWidth / defaultWidth, (float)viewportHeight / defaultHeight);
                pixelsPerUnit = defaultPixelsPerUnit * multiplier;
                graphicsScale = defaultGraphicsScale * multiplier;
            }
        }

        static bool fullScreen = defaultFullScreen;
        /// <summary>
        /// Whether the game is launched in full screen or not.
        /// </summary>
        public static bool FULL_SCREEN { get { return fullScreen; } set { fullScreen = value; } }
        
        /// <summary>
        /// Epsilon constant used occassionally in the graphics engine.
        /// </summary>
        public const float EPSILON = 0.001f;
        /// <summary>
        /// The default smoothness of the camera's movement.
        /// </summary>
        public const float DEFAULT_CAMERA_SMOOTHNESS = 0.05f;
        /// <summary>
        /// The camera's alternate zoom upon pressing the zoom key.
        /// </summary>
        public const float CAMERA_ALT_ZOOM = 0.5f;
        /// <summary>
        /// The camera's moving speed while pressing the zoom key.
        /// </summary>
        public const float CAMERA_SPEED = 0.5f;
        /// <summary>
        /// The seamstress's default animation speed.
        /// </summary>
        public const int DEFAULT_ANIMATION_SPEED = 3;
        /// <summary>
        /// The amount of bias towards the viewer certain objects have.
        /// </summary>
        public const float VIEWER_TILT = 0.075f;

        public const float MIASMA_DAMPING = 0.99f;
    }
}
