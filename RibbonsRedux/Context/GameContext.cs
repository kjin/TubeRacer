using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RibbonsRedux.Graphics;
using RibbonsRedux.Audio;
using RibbonsRedux.Input;
using RibbonsRedux.Content;
using RibbonsRedux.Storage;

namespace RibbonsRedux.Context
{
    /// <summary>
    /// Represents one of several contexts of the game. Possible contexts include menus and the actual gameplay itself.
    /// </summary>
    public abstract class GameContext
    {
        public AssetManager AssetManager;
        public InputController InputController;
        public Canvas Canvas;
        public AudioPlayer AudioPlayer;
        public DataCenter DataCenter;

        float gridWidth;
        float gridHeight;
        bool gridChanged;

        GameContext nextState;
        bool exit;
        Color backgroundColor;
        float fadeMultiplier;

        TimeSpan timeLoaded;
        bool timeSet;

        static float DEFAULT_FADE_MULTIPLIER = 0.2f;

        public GameContext NextContext { get { return nextState; } set { nextState = value; } }

        public bool Exit { get { return exit; } set { exit = value; } }

        /// <summary>
        /// Gets or sets the background color of this game context.
        /// </summary>
        public Color BackgroundColor { get { return backgroundColor; } protected set { backgroundColor = value; } }
        /// <summary>
        /// Gets or sets the fade in/out multiplier. The higher this number is, the faster the context will fade in or out.
        /// </summary>
        public float FadeMultiplier { get { return fadeMultiplier; } protected set { fadeMultiplier = value; } }

        public float GridWidth { get { return gridWidth; } set { gridWidth = value; } }
        public float GridHeight { get { return gridHeight; } set { gridHeight = value; } }
        public bool GridChanged { get { return gridChanged; } set { gridChanged = value; } }

        public TimeSpan TimeLoaded
        {
            get
            {
                if (timeSet)
                    return timeLoaded;
                else
                    return TimeSpan.MaxValue; //just a really long timespan.
            }
        }

        /// <summary>
        /// Constucts a new GameContext object.
        /// </summary>
        public GameContext()
        {
            this.fadeMultiplier = DEFAULT_FADE_MULTIPLIER;
            timeSet = false;
        }

        /// <summary>
        /// Initialize the current context.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Frees memory allocated during the creation of this instance.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Updates the context state.
        /// </summary>
        /// <param name="gameTime">A snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (!timeSet)
            {
                timeLoaded = gameTime.TotalGameTime;
                timeSet = true;
            }
        }
        /// <summary>
        /// Draws objects in this context.
        /// </summary>
        /// <param name="gameTime">A snapshot of timing values.</param>
        public abstract void Draw(GameTime gameTime);
        /// <summary>
        /// Plays sounds in this context.
        /// </summary>
        /// <param name="gameTime">A snapshot of timing values.</param>
        public abstract void PlayAudio(GameTime gameTime);
    }
}
