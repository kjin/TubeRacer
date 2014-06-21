using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RibbonsRedux.Content;

namespace RibbonsRedux.Graphics
{
    /// <summary>
    /// A class that exposes relevant image dimensions for physical objects, and abstracts animation.
    /// </summary>
    public class Sprite
    {
        Texture2D texture;
        protected float scale;
        protected float width;
        protected float height;
        protected int frameWidth;
        protected int frameHeight;
        protected int rows;
        protected int columns;
        protected int frames;
        protected bool once;

        protected int currentFrame;
        protected int tickCounter;
        protected int ticksPerFrame;
        protected float animationProgress;
        /// <summary>
        /// Create a new animated SimpleSprite object, where frames are arranged on a grid in row-major order.
        /// For non-animated sprites, only the first argument should be specified.
        /// For animated sprites on a single filmstrip, the rows parameter may be omitted.
        /// </summary>
        /// <param name="texture">The texture on which this sprite is based.</param>
        /// <param name="columns">The number of columns of frames in the texture.</param>
        /// <param name="rows">The number of rows of frames in the texture.</param>
        protected Sprite(Texture2D texture, int columns = 1, int rows = 1, bool once = false, float scale = 1)
        {
            this.scale = scale;
            scale /= GraphicsConstants.SPRITE_SCALE;
            this.texture = texture;
            this.frameWidth = texture.Width / columns;
            this.frameHeight = texture.Height / rows;
            this.width = frameWidth / GraphicsConstants.DEFAULT_PIXELS_PER_UNIT / scale;
            this.height = frameHeight / GraphicsConstants.DEFAULT_PIXELS_PER_UNIT / scale;
            this.rows = rows;
            this.columns = columns;
            this.frames = rows * columns;
            this.once = once;

            Reset();
            ticksPerFrame = 1;
        }

        /// <summary>
        /// Creates an identical copy of this Sprite object.
        /// </summary>
        /// <returns>A copy of this Sprite object.</returns>
        public Sprite Clone()
        {
            Sprite s = new Sprite(texture, columns, rows, once, scale);
            s.currentFrame = currentFrame;
            s.tickCounter = tickCounter;
            s.ticksPerFrame = ticksPerFrame;
            s.animationProgress = animationProgress;
            return s;
        }

        /// <summary>
        /// Gets the width of the sprite in physical coordinates.
        /// </summary>
        public float Width { get { return width; } }
        /// <summary>
        /// Gets the height of the sprite in physical coordinates.
        /// </summary>
        public float Height { get { return height; } }
        /// <summary>
        /// Gets the number of frames in the sprite.
        /// </summary>
        public int Frames { get { return frames; } }
        /// <summary>
        /// Gets the number of rows in the sprite.
        /// </summary>
        public int Rows { get { return rows; } }
        /// <summary>
        /// Gets the number of columns in the sprite.
        /// </summary>
        public int Columns { get { return columns; } }

        /// <summary>
        /// Gets the number of ticks between frames.
        /// </summary>
        public int TicksPerFrame { get { return ticksPerFrame; } set { ticksPerFrame = value; } }
        /// <summary>
        /// Gets or sets the current frame.
        /// </summary>
        public int CurrentFrame { get { return currentFrame; } set { currentFrame = value; } }
        /// <summary>
        /// Gets whether the animation has finished.
        /// </summary>
        public bool AnimationDone { get { return animationProgress == 1f; } }
        /// <summary>
        /// Gets the animation progress.
        /// </summary>
        public float AnimationProgress { get { return animationProgress; } }

        public void Reset()
        {
            currentFrame = 0;
            tickCounter = 0;
            animationProgress = 0;
        }

        public void Tick()
        {
            tickCounter++;
            if (ticksPerFrame > 0 && tickCounter >= ticksPerFrame)
            {
                if (!once || currentFrame < Frames - 1)
                {
                    currentFrame = (currentFrame + 1) % Frames;
                    animationProgress = (float)currentFrame / Frames;
                }
                else
                    animationProgress = 1f;
                tickCounter = 0;
            }
        }

        /// <summary>
        /// Returns a bounding rectangle for the specified frame.
        /// </summary>
        /// <param name="num">The frame to target.</param>
        /// <returns>The bounding retangle for the specified frame.</returns>
        public Rectangle GetFrame(int num)
        {
            if (num >= Frames || num < 0) throw new ArgumentOutOfRangeException("Frame number is out of range.");
            return new Rectangle(frameWidth * (num % columns), frameHeight * (num / columns), frameWidth, frameHeight);
        }

        /// <summary>
        /// Gets the texture associated with this sprite.
        /// Note: the dimensions of this texture must not be used in any physics calculations.
        /// Use Sprite.Width and Sprite.Height instead.
        /// </summary>
        public Texture2D Texture { get { return texture; } }

        public static Sprite Build(AssetManager assets, string assetName, int startFrame = 0)
        {
            int columns = 1;
            int rows = 1;
            bool once = false;
            TextDictionary assetDictionary = new TextDictionary(assets.GetText("sprites"));
            if (assetDictionary.CheckPropertyExists(assetName, "columns"))
                columns = assetDictionary.LookupInt32(assetName, "columns");
            if (assetDictionary.CheckPropertyExists(assetName, "rows"))
                rows = assetDictionary.LookupInt32(assetName, "rows");
            if (assetDictionary.CheckPropertyExists(assetName, "once"))
                once = assetDictionary.LookupBoolean(assetName, "once");
            Sprite sprite = new Sprite(assets.GetTexture(assetName), columns, rows, once);
            sprite.currentFrame = startFrame;
            return sprite;
        }
    }

    /*public class MultiSprite : Sprite
    {
        protected float width;
        protected float height;
        protected LinkedList<Tuple<string, SimpleSprite>>[] sprites;
        protected int numSprites;

        public MultiSprite()
        {
            sprites = new LinkedList<Tuple<string, SimpleSprite>>[26];
            for (int i = 0; i < sprites.Length; i++)
                sprites[i] = new LinkedList<Tuple<string, SimpleSprite>>();
            numSprites = 0;
        }

        public void AddSimpleSprite(string identifier, Texture2D texture, int columns = 1, int rows = 1)
        {
            int index = identifier[0] - 'a';
            SimpleSprite ss = new SimpleSprite(texture, columns, rows);
            sprites[index].AddFirst(new Tuple<string, SimpleSprite>(identifier, ss));
            if (numSprites > 0)
            {
                width = ss.Width;
                height = ss.Height;
            }
            numSprites++;
        }

        public SimpleSprite GetSimpleSprite(string identifier)
        {
            int index = identifier[0] - 'a';
            if (sprites[index].Count == 1)
                return sprites[index].First.Value.Item2;
            else if (sprites[index].Count > 1)
            {
                LinkedListNode<Tuple<string, SimpleSprite>> itr = sprites[index].First;
                while (itr.Next != null)
                {
                    if (itr.Value.Item1.Equals(identifier))
                        return itr.Value.Item2;
                    itr = itr.Next;
                }
            }
            //not found
            throw new ArgumentException("No sprite exists under that identifier.");
        }
        
        /// <summary>
        /// Gets the width of the sprite in physical coordinates.
        /// </summary>
        public float Width { get { return width; } }
        /// <summary>
        /// Gets the height of the sprite in physical coordinates.
        /// </summary>
        public float Height { get { return height; } }
    }*/
}
