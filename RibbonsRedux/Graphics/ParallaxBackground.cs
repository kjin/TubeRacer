using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RibbonsRedux.Content;

namespace RibbonsRedux.Graphics
{
    //Represents a single scrolling background object.
    public class ParallaxBackground
    {
        Texture2D texture;
        Vector2 initialOffset;
        Vector2 velocity;
        Anchor anchor;
        float dist;
        float scale;
        bool repeatX;
        bool repeatY;

        public ParallaxBackground(AssetManager assets, Texture2D texture, Vector2 initialOffset, Vector2 velocity, float dist, float scale, Anchor anchor, Vector2 repeat)
        {
            this.texture = texture;
            this.dist = dist;
            this.initialOffset = initialOffset - (GraphicsHelper.ComputeAnchorOrigin(anchor, new Vector2(texture.Width, texture.Height) / GraphicsConstants.VIEWPORT_DIMENSIONS)) + GraphicsHelper.ComputeAnchorOrigin(anchor, Vector2.One);
            this.anchor = anchor;
            this.velocity = velocity;
            this.scale = scale;
            this.repeatX = repeat.X != 0;
            this.repeatY = repeat.Y != 0;
        }

        /// <summary>
        /// Gets the texture associated with the object.
        /// </summary>
        public Texture2D Texture { get { return texture; } }
        /// <summary>
        /// Gets the distance from the camera, which is factored into how quickly the background scrolls.
        /// </summary>
        public float Distance { get { return dist; } }
        /// <summary>
        /// Gets the initial offset, which is where the background should be drawn when the camera is at the origin.
        /// </summary>
        public Vector2 InitialOffset { get { return initialOffset; } }
        /// <summary>
        /// Gets the velocity, which is how quickly the background should move.
        /// </summary>
        public Vector2 Velocity { get { return velocity; } }
        /// <summary>
        /// Gets the anchor position of the backgrounds.
        /// </summary>
        public Anchor Anchor { get { return anchor; } }
        /// <summary>
        /// Gets the scale factor to apply to the background when drawing.
        /// </summary>
        public float Scale { get { return scale; } }
        /// <summary>
        /// Gets whether the background repeats in the X direction.
        /// </summary>
        public bool RepeatX { get { return repeatX; } }
        /// <summary>
        /// Gets whether the background repeats in the Y direction.
        /// </summary>
        public bool RepeatY { get { return repeatY; } }
    }

    public class ParallaxBackgroundSet
    {
        ParallaxBackground[] backgrounds;
        BackgroundParticleSet particles;
        Camera camera;
        Color color;
        int particleLayer;
        Vector2 seamstressStart;

        public static ParallaxBackgroundSet Build(AssetManager assets, Camera camera, Vector2 seamstressStart, string assetName)
        {
            TextDictionary assetDictionary = new TextDictionary(assets.GetText("parallax"));
            int layers = assetDictionary.LookupInt32(assetName, "layers");
            ParallaxBackground[] backgrounds = new ParallaxBackground[layers];
            BackgroundParticleSet particles = new BackgroundParticleSet(assets, camera.Dimensions, Convert.ToInt32(assetName.Substring(3)));
            int particleLayer = -1;
            for (int i = 0; i < layers; i++)
            {
                Texture2D tex;
                if (assetDictionary.CheckPropertyExists(assetName, "name" + i))
                    tex = assets.GetTexture(assetDictionary.LookupString(assetName, "name" + i));
                else
                    tex = assets.GetTexture(assetName + "_layer" + i);
                float dist, scale;
                Vector2 offset, velocity, repeat;
                Anchor anchor;
                try { dist = assetDictionary.LookupSingle(assetName, "dist" + i); }
                catch { dist = 1; }
                try { offset = assetDictionary.LookupVector2(assetName, "offset" + i); }
                catch { offset = Vector2.Zero; }
                try { velocity = assetDictionary.LookupVector2(assetName, "velocity" + i); }
                catch { velocity = Vector2.Zero; }
                try { scale = assetDictionary.LookupSingle(assetName, "scale" + i); }
                catch { scale = 1; }
                try { repeat = assetDictionary.LookupVector2(assetName, "repeat" + i); }
                catch { repeat = new Vector2(1, 0); }
                if (!assetDictionary.CheckPropertyExists(assetName, "anchor" + i) ||
                    !Enum.TryParse<Anchor>(assetDictionary.LookupString(assetName, "anchor" + i), out anchor))
                    anchor = Anchor.BottomLeft;
                backgrounds[i] = new ParallaxBackground(assets, tex, offset, velocity, dist, scale, anchor, repeat);
                if (dist > particles.Distance)
                    particleLayer = i;
            }
            ParallaxBackgroundSet pbs = new ParallaxBackgroundSet();
            pbs.backgrounds = backgrounds;
            pbs.camera = new Camera(camera.Dimensions, camera.Position, camera.Rotation, camera.Scale);
            pbs.color = assetDictionary.LookupColor(assetName, "color");
            pbs.particleLayer = particleLayer;
            pbs.particles = particles;
            pbs.seamstressStart = seamstressStart;

            return pbs;
        }

        public void Update(GameTime gameTime)
        {
            particles.Update(camera.ActualPosition);
        }

        public void Draw(GameTime gameTime, Canvas canvas)
        {
            canvas.PushCamera(camera);
            if (particleLayer == -1)
                particles.Draw(canvas);
            Vector2 offset = (camera.ActualPosition - seamstressStart) / 2;
            offset.Y = 0;
            for (int i = 0; i < backgrounds.Length; i++)
            {
                Vector2 position = backgrounds[i].InitialOffset * camera.Dimensions - offset / backgrounds[i].Distance + backgrounds[i].Velocity * (float)gameTime.TotalGameTime.TotalSeconds;
                Vector2 increment = new Vector2(backgrounds[i].Texture.Width / GraphicsConstants.PIXELS_PER_UNIT, backgrounds[i].Texture.Height / GraphicsConstants.PIXELS_PER_UNIT);
                if (backgrounds[i].RepeatX)
                {
                    while (position.X < -increment.X)
                        position.X += increment.X;
                    while (position.X > 0)
                        position.X -= increment.X;
                }
                if (backgrounds[i].RepeatY)
                {
                    while (position.Y < -increment.Y)
                        position.Y += increment.Y;
                    while (backgrounds[i].RepeatY && position.Y > 0)
                        position.Y -= increment.Y;
                }
                while (position.X <= camera.Dimensions.X)
                {
                    float cachedY = position.Y;
                    while (position.Y <= camera.Dimensions.Y)
                    {
                        canvas.DrawTexture(backgrounds[i].Texture, Color.White, position * GraphicsConstants.GRAPHICS_SCALE, Anchor.TopLeft);
                        if (backgrounds[i].RepeatY)
                            position.Y += increment.Y;
                        else break;
                    }
                    position.Y = cachedY;
                    if (backgrounds[i].RepeatX)
                        position.X += increment.X;
                    else break;
                }
                if (particleLayer == i)
                    particles.Draw(canvas);
            }
            canvas.PopCamera();
        }

        public Color Color { get { return color; } }
    }

}
