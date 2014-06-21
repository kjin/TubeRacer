using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RibbonsRedux.Content;

namespace RibbonsRedux.Graphics
{
    /// <summary>
    /// The main graphics engine for the game.
    /// </summary>
    public class Canvas
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        List<Camera> cameraStack;
        Camera basicCamera;

        bool debug;

        //Temporary SpriteFont
        SpriteFont spriteFont;

        //1x1 texture used in drawing lines and boxes
        Texture2D square1x1;

        float layerDepth;
        bool frozen;
        Vector2 offset;

        public float Time;

        //2D transformation matrix.
        Matrix transformationMatrix;

        static float LAYER_DEPTH_DEFAULT = 0f;

        /// <summary>
        /// Create a new Canvas object in screen coordinate mode.
        /// </summary>
        /// <param name="graphics">The GraphicsDeviceManager used in the game.</param>
        /// <param name="graphicsDevice">The game's GraphicsDevice object.</param>
        public Canvas(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice)
        {
            Time = 0;
            //create camera stack
            cameraStack = new List<Camera>(8);
            //add the canvas's default camera
            basicCamera = new Camera(Vector2.One, Vector2.Zero);
            cameraStack.Add(basicCamera);
            //change size of window
            this.graphics = graphics;
            SetViewParameters();

            debug = false;

            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);

            // initialize our tiny square. This square is used for drawing primitives.
            square1x1 = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] data = new Color[1];
            data[0] = Color.White;
            square1x1.SetData<Color>(data);

            frozen = false;
            layerDepth = LAYER_DEPTH_DEFAULT;

            transformationMatrix = Matrix.Identity;
        }

        public void SetViewParameters()
        {
            graphics.PreferredBackBufferWidth = GraphicsConstants.VIEWPORT_WIDTH;
            graphics.PreferredBackBufferHeight = GraphicsConstants.VIEWPORT_HEIGHT;
            graphics.IsFullScreen = GraphicsConstants.FULL_SCREEN;
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Loads content for the canvas.
        /// </summary>
        /// <param name="assets">The AssetManager used to load content.</param>
        public void LoadContent(AssetManager assets)
        {
            TextDictionary assetDictionary = new TextDictionary(assets.GetText("graphics"));
            spriteFont = assets.GetFont(assetDictionary.LookupString("canvas", "internalFont"));
        }

        #region Begin/end draw calls
        /// <summary>
        /// Starts a spriteBatch pass.
        /// </summary>
        public void BeginDraw()
        {
            spriteBatch.Begin(0, null, null, null, null, null, transformationMatrix);
        }

        /// <summary>
        /// Starts a spriteBatch pass.
        /// </summary>
        public void BeginDraw(SpriteSortMode spriteSortMode = 0, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null)
        {
            spriteBatch.Begin(spriteSortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformationMatrix);
        }

        /// <summary>
        /// Ends a spriteBatch pass.
        /// </summary>
        public void EndDraw() { spriteBatch.End(); }

        public void Begin3D()
        {
            graphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        public void End3D()
        {
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
        #endregion

        #region 3D drawing methods
        /// <summary>
        /// Draws a mesh defined by a list of vertices and vertex indices.
        /// </summary>
        /// <typeparam name="T">The vertex declaration type of the vertices.</typeparam>
        /// <param name="effect">The effect to be used. This effect must contain the following parameters: World, View, and Projection (float4x4), as well as Time (float).</param>
        /// <param name="mesh">The mesh to be drawn.</param>
        public void DrawUserIndexedPrimitives<T>(Effect effect, UserIndexedPrimitives<T, short> mesh) where T : struct, IVertexType
        {
            effect.Parameters["World"].SetValue(ActiveCamera.World);
            effect.Parameters["View"].SetValue(Matrix.CreateTranslation(offset.X, offset.Y, 0) * ActiveCamera.View);
            effect.Parameters["Projection"].SetValue(ActiveCamera.Projection);
            if (!frozen)
                mesh.Tick();
            Time = mesh.Ticks;
            foreach (EffectPass e in effect.CurrentTechnique.Passes)
                e.Apply();
            int div = 3;
            if (mesh.PrimitiveType == PrimitiveType.LineList || mesh.PrimitiveType == PrimitiveType.LineStrip)
                div = 2;
            graphicsDevice.DrawUserIndexedPrimitives<T>(mesh.PrimitiveType, mesh.Vertices, 0, mesh.Vertices.Length, mesh.Indices, 0, mesh.Indices.Length / div);
        }
        #endregion

        #region Texture drawing methods
        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Color color, Vector2 position, float rotation = 0, float scale = 1, bool flip = false)
        {
            DrawTexture(texture, color, position, Anchor.Center, rotation, scale, flip);
        }

        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="sourceRectangle">The portion of the texture to draw.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Rectangle sourceRectangle, Color color, Vector2 position, float rotation = 0, float scale = 1, bool flip = false)
        {
            DrawTexture(texture, sourceRectangle, color, position, Anchor.Center, rotation, scale, flip);
        }

        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Color color, Vector2 position, Anchor anchor, float rotation = 0, float scale = 1, bool flip = false)
        {
            if (texture == null) return;
            Vector2 origin = Vector2.Zero;
            Vector2 dimensions = new Vector2(texture.Width, texture.Height);
            if (anchor != Anchor.TopLeft)
                origin = GraphicsHelper.ComputeAnchorOrigin(anchor, dimensions);
            SpriteEffects effect = SpriteEffects.None;
            if (flip)
                effect = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, ActiveCamera.Transform(position) + offset, null, color, rotation - ActiveCamera.ActualRotation, origin, scale * ActiveCamera.ActualScale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
            //Console.WriteLine("Drew a texture at {0} with width {1} and height {2}.", ActiveCamera.Transform(position), texture.Width, texture.Height);
        }

        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Color color, Vector2 position, Anchor anchor, float rotation, Vector2 scale, bool flip = false)
        {
            if (texture == null) return;
            Vector2 origin = Vector2.Zero;
            Vector2 dimensions = new Vector2(texture.Width, texture.Height);
            if (anchor != Anchor.TopLeft)
                origin = GraphicsHelper.ComputeAnchorOrigin(anchor, dimensions);
            SpriteEffects effect = SpriteEffects.None;
            if (flip)
                effect = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, ActiveCamera.Transform(position) + offset, null, color, rotation - ActiveCamera.ActualRotation, origin, scale * ActiveCamera.ActualScale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
        }

        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="sourceRectangle">The portion of the texture to draw.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Rectangle sourceRectangle, Color color, Vector2 position, Anchor anchor, float rotation = 0, float scale = 1, bool flip = false)
        {
            if (texture == null) return;
            Vector2 origin = Vector2.Zero;
            Vector2 dimensions = new Vector2(sourceRectangle.Width, sourceRectangle.Height);
            if (anchor != Anchor.TopLeft)
                origin = GraphicsHelper.ComputeAnchorOrigin(anchor, dimensions);
            SpriteEffects effect = SpriteEffects.None;
            if (flip)
                effect = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, ActiveCamera.Transform(position) + offset, sourceRectangle, color, rotation - ActiveCamera.ActualRotation, origin, scale * ActiveCamera.ActualScale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
        }

        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Rectangle sourceRectangle, Color color, Vector2 position, Anchor anchor, float rotation, Vector2 scale, bool flip = false)
        {
            if (texture == null) return;
            Vector2 origin = Vector2.Zero;
            Vector2 dimensions = new Vector2(sourceRectangle.Width, sourceRectangle.Height);
            if (anchor != Anchor.TopLeft)
                origin = GraphicsHelper.ComputeAnchorOrigin(anchor, dimensions);
            SpriteEffects effect = SpriteEffects.None;
            if (flip)
                effect = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, ActiveCamera.Transform(position), sourceRectangle, color, rotation - ActiveCamera.ActualRotation, origin, scale * ActiveCamera.ActualScale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
        }
        #endregion

        #region Sprite drawing methods
        /// <summary>
        /// Draws a sprite to the screen based on the canvas's coordinate mode. Generally, this should be the method used
        /// for drawing physical objects.
        /// </summary>
        /// <param name="sprite">The sprite to draw. If the sprite is animated, only the first frame will be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        public void DrawSprite(Sprite sprite, Color color, Vector2 position, float rotation = 0, float scale = 1, bool flip = false)
        {
            DrawSprite(sprite, color, position, rotation, scale * Vector2.One, flip);
        }

        /// <summary>
        /// Draws a sprite to the screen based on the canvas's coordinate mode. This method should only be called for physical
        /// objects that for some reason aren't positioned at their centers.
        /// </summary>
        /// <param name="sprite">The sprite to draw. If the sprite is animated, only the first frame will be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        public void DrawSprite(Sprite sprite, Color color, Vector2 position, Anchor anchor, float rotation = 0, float scale = 1, bool flip = false)
        {
            DrawSprite(sprite, color, position, anchor, rotation, scale * Vector2.One, flip);
        }

        /// <summary>
        /// Draws a sprite to the screen based on the canvas's coordinate mode. Generally, this should be the method used
        /// for drawing physical objects.
        /// </summary>
        /// <param name="sprite">The sprite to draw. If the sprite is animated, only the first frame will be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        public void DrawSprite(Sprite sprite, Color color, Vector2 position, float rotation, Vector2 scale, bool flip = false)
        {
            DrawSprite(sprite, color, position, Anchor.Center, rotation, scale, flip);
        }

        /// <summary>
        /// Draws a sprite to the screen based on the canvas's coordinate mode. This method should only be called for physical
        /// objects that for some reason aren't positioned at their centers.
        /// </summary>
        /// <param name="sprite">The sprite to draw. If the sprite is animated, only the first frame will be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        public void DrawSprite(Sprite sprite, Color color, Vector2 position, Anchor anchor, float rotation, Vector2 scale, bool flip = false)
        {
            if (!frozen)
                sprite.Tick();
            DrawTexture(sprite.Texture, sprite.GetFrame(sprite.CurrentFrame), color, position, anchor,
                                                                                    rotation, GraphicsConstants.SPRITE_SCALE * scale, flip);
        }
        #endregion

        #region Text drawing methods
        /// <summary>
        /// Draws a string of text to the screen based on the canvas's coordinate mode, using the canvas's internal font.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The position at which the text should be written.</param>
        /// <param name="rotation">How much the text is rotated.</param>
        /// <param name="scale">The scale at which the text is written.</param>
        public void DrawString(object text, Color color, Vector2 position, float rotation = 0, float scale = 1)
        {
            DrawString(text, spriteFont, color, position, rotation, scale);
        }

        /// <summary>
        /// Draws a string of text to the screen based on the canvas's coordinate mode, using the canvas's internal font.
        /// Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The position at which the text should be written.</param>
        /// <param name="rotation">How much the text is rotated.</param>
        /// <param name="scale">The scale at which the text is written.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        public void DrawString(object text, Color color, Vector2 position, Anchor anchor, float rotation = 0, float scale = 1)
        {
            DrawString(text, spriteFont, color, position, anchor, rotation, scale);
        }

        /// <summary>
        /// Draws a string of text to the screen based on the canvas's coordinate mode.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font to use.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The position at which the text should be written.</param>
        /// <param name="rotation">How much the text is rotated.</param>
        /// <param name="scale">The scale at which the text is written.</param>
        public void DrawString(object text, SpriteFont font, Color color, Vector2 position, float rotation = 0, float scale = 1)
        {
            DrawString(text, font, color, position, Anchor.Center, rotation, scale);
        }

        /// <summary>
        /// Draws a string of text to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font to use.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The position at which the text should be written.</param>
        /// <param name="rotation">How much the text is rotated.</param>
        /// <param name="scale">The scale at which the text is written.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        public void DrawString(object text, SpriteFont font, Color color, Vector2 position, Anchor anchor, float rotation = 0, float scale = 1)
        {
            String objectText = text.ToString();
            Vector2 origin = Vector2.Zero;
            Vector2 dimensions = font.MeasureString(objectText);
            if (anchor != Anchor.TopLeft)
                origin = GraphicsHelper.ComputeAnchorOrigin(anchor, dimensions);
            spriteBatch.DrawString(font, objectText, ActiveCamera.Transform(position) + offset, color, rotation - ActiveCamera.ActualRotation, origin, scale * ActiveCamera.ActualScale, SpriteEffects.None, 0);
        }
        #endregion

        #region Debug drawing methods
        /// <summary>
        /// Draws a line on the screen.
        /// </summary>
        /// <param name="color">The color of the line.</param>
        /// <param name="thickness">The thickness of the line, in pixels.</param>
        /// <param name="p1">The starting point of the line, in the canvas's current coordinate mode.</param>
        /// <param name="p2">The ending point of the line, in the canvas's current coordinate mode.</param>
        /// <param name="debug">Whether the line should only be drawn in debug mode.</param>
        public void DrawLine(Color color, float thickness, Vector2 p1, Vector2 p2, bool debug = true)
        {
            if (!DebugMode && debug) return;
            Vector2 diff = p2 - p1;
            float angle = (float)Math.Atan2(diff.Y, diff.X);
            float length = diff.Length();
            length *= GraphicsConstants.PIXELS_PER_UNIT * ActiveCamera.ActualScale;
            spriteBatch.Draw(square1x1, ActiveCamera.Transform(p1) + offset, null, color, angle - ActiveCamera.ActualRotation, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }


        /// <summary>
        /// Draws the outline of a polygon on the screen.
        /// </summary>
        /// <param name="color">The color of the outline.</param>
        /// <param name="thickness">The thickness of the outline, in pixels.</param>
        /// <param name="points">The points that make up this polygon.</param>
        /// <param name="debug">Whether the polygon should only be drawn in debug mode.</param>
        public void DrawPolygon(Color color, float thickness, List<Vector2> points, bool debug = true)
        {
            for (int i = 0; i < points.Count; i++)
                DrawLine(color, thickness, points[i], points[(i + 1) % points.Count], debug);
        }

        /// <summary>
        /// Draws an axis-aligned rectangle on the screen.
        /// </summary>
        /// <param name="border">The color of the rectangle's border.</param>
        /// <param name="borderThickness">The thickness of the rectangle's border.</param>
        /// <param name="rect">The rectangle's dimensions.</param>
        /// <param name="debug">Whether the rectangle should only be drawn in debug mode.</param>
        public void DrawRectangle(Color border, int borderThickness, Rectangle rect, bool debug = true)
        {
            DrawRectangle(border, Color.Transparent, borderThickness, new Vector2(rect.X, rect.Y), Anchor.TopLeft, new Vector2(rect.Width, rect.Height), 0, debug);
        }

        /// <summary>
        /// Draws an axis-aligned rectangle on the screen.
        /// </summary>
        /// <param name="border">The color of the rectangle's border.</param>
        /// <param name="fill">The color of the rectangle's fill. Use Color.Transparent to prevent the rectangle from being filled.</param>
        /// <param name="borderThickness">The thickness of the rectangle's border.</param>
        /// <param name="rect">The rectangle's dimensions.</param>
        /// <param name="debug">Whether the rectangle should only be drawn in debug mode.</param>
        public void DrawRectangle(Color border, Color fill, int borderThickness, Rectangle rect, bool debug = true)
        {
            DrawRectangle(border, fill, borderThickness, new Vector2(rect.X, rect.Y), Anchor.TopLeft, new Vector2(rect.Width, rect.Height), 0, debug);
        }

        /// <summary>
        /// Draws a rectangle with no fill on the screen.
        /// </summary>
        /// <param name="border">The color of the rectangle's border.</param>
        /// <param name="borderThickness">The thickness of the rectangle's border.</param>
        /// <param name="position">The position of the rectangle.</param>
        /// <param name="size">The size of the rectangle, in the canvas's current coordinate mode.</param>
        /// <param name="rotation">The rotation the rectangle.</param>
        /// <param name="debug">Whether the rectangle should only be drawn in debug mode.</param>
        public void DrawRectangle(Color border, int borderThickness, Vector2 position, Vector2 size, float rotation = 0, bool debug = true)
        {
            DrawRectangle(border, Color.Transparent, borderThickness, position, size, rotation, debug);
        }

        /// <summary>
        /// Draws a rectangle on the screen.
        /// </summary>
        /// <param name="border">The color of the rectangle's border.</param>
        /// <param name="fill">The color of the rectangle's fill. Use Color.Transparent to prevent the rectangle from being filled.</param>
        /// <param name="borderThickness">The thickness of the rectangle's border.</param>
        /// <param name="position">The position of the rectangle.</param>
        /// <param name="size">The size of the rectangle, in the canvas's current coordinate mode.</param>
        /// <param name="rotation">The rotation the rectangle.</param>
        /// <param name="debug">Whether the rectangle should only be drawn in debug mode.</param>
        public void DrawRectangle(Color border, Color fill, int borderThickness, Vector2 position, Vector2 size, float rotation = 0, bool debug = true)
        {
            DrawRectangle(border, fill, borderThickness, position, Anchor.Center, size, rotation, debug);
        }

        /// <summary>
        /// Draws a rectangle on the screen. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="border">The color of the rectangle's border.</param>
        /// <param name="fill">The color of the rectangle's fill. Use Color.Transparent to prevent the rectangle from being filled.</param>
        /// <param name="borderThickness">The thickness of the rectangle's border.</param>
        /// <param name="position">The position of the rectangle.</param>
        /// <param name="size">The size of the rectangle, in the canvas's current coordinate mode.</param>
        /// <param name="rotation">The rotation the rectangle.</param>
        /// <param name="debug">Whether the rectangle should only be drawn in debug mode.</param>
        public void DrawRectangle(Color border, Color fill, int borderThickness, Vector2 position, Anchor anchor, Vector2 size, float rotation = 0, bool debug = true)
        {
            if (!DebugMode && debug) return;
            Vector2 dimensions = GraphicsHelper.ComputeAnchorOrigin(anchor, size);
            float cos = (float)Math.Cos(rotation);
            float sin = (float)Math.Sin(rotation);
            Vector2 p1 = -dimensions;
            p1 = new Vector2(position.X + cos * p1.X - sin * p1.Y, position.Y + sin * p1.X + cos * p1.Y);
            Vector2 p2 = new Vector2(size.X, 0) - dimensions;
            p2 = new Vector2(position.X + cos * p2.X - sin * p2.Y, position.Y + sin * p2.X + cos * p2.Y);
            Vector2 p3 = size - dimensions;
            p3 = new Vector2(position.X + cos * p3.X - sin * p3.Y, position.Y + sin * p3.X + cos * p3.Y);
            Vector2 p4 = new Vector2(0, size.Y) - dimensions;
            p4 = new Vector2(position.X + cos * p4.X - sin * p4.Y, position.Y + sin * p4.X + cos * p4.Y);
            if (fill.A != 0)
            {
                Vector2 recScale = size * ActiveCamera.ActualScale * GraphicsConstants.VIEWPORT_DIMENSIONS;
                spriteBatch.Draw(square1x1, ActiveCamera.Transform(p1) + offset, null, fill, rotation - ActiveCamera.ActualRotation, Vector2.Zero, recScale, SpriteEffects.None, 0);
            }

            DrawLine(border, borderThickness, p1, p2, debug);
            DrawLine(border, borderThickness, p2, p3, debug);
            DrawLine(border, borderThickness, p3, p4, debug);
            DrawLine(border, borderThickness, p4, p1, debug);
        }
        #endregion

        #region Properties
        
        /// <summary>
        /// Whether the canvas should stop all animations from advancing.
        /// </summary>
        public bool Frozen { get { return frozen; } set { frozen = value; } }

        /// <summary>
        /// The translational offset with which everything is drawn.
        /// TODO: Remove this; it's too hacky.
        /// </summary>
        public Vector2 Offset { get { return offset; } set { offset = value; } }

        /// <summary>
        /// Gets or sets whether the canvas draws debug mode objects.
        /// </summary>
        public bool DebugMode { get { return debug; } set { debug = value; } }

        /// <summary>
        /// Gets the active camera.
        /// </summary>
        public Camera ActiveCamera
        {
            get { return cameraStack.Count == 0 ? null : cameraStack[cameraStack.Count - 1]; }
        }

        /// <summary>
        /// Gets the basic camera associated with this canvas. This camera draws objects with no transformation.
        /// </summary>
        public Camera BasicCamera
        {
            get { return basicCamera; }
        }

        // Sorry for the douchey one-liners
        public void PushCamera(Camera camera) { cameraStack.Add(camera); }
        public void PopCamera() { if (cameraStack.Count == 0) return; cameraStack.RemoveAt(cameraStack.Count - 1); }
        public bool ResetCameraStack() { if (cameraStack.Count <= 1) return false; while (cameraStack.Count > 1) PopCamera(); return true; }
        #endregion
    }
}