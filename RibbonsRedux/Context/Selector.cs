using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RibbonsRedux.Graphics;
using RibbonsRedux.Audio;
using RibbonsRedux.Input;

namespace RibbonsRedux.Context
{
    /// <summary>
    /// An abstract class that represents a visual set of selectable options.
    /// </summary>
    public abstract class Selector
    {
        string themeName;
        float floatValue;
        int intValue;
        bool boolValue;
        Vector2 position;
        Camera absoluteCamera;

        public Selector(string themeName, Vector2 position, bool initialValue) : this(themeName, position, initialValue ? 1f : 0f) { }

        public Selector(string themeName, Vector2 position, int initialValue) : this(themeName, position, (float)initialValue) { }

        public Selector(string themeName, Vector2 position, float initialValue)
        {
            Position = position;
            this.themeName = themeName;
            this.floatValue = initialValue;
            this.intValue = (int)initialValue;
            this.boolValue = initialValue != 0f;
        }

        /// <summary>
        /// Gets or sets the position of the selector.
        /// </summary>
        protected Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                absoluteCamera = new Camera(GraphicsConstants.VIEWPORT_DIMENSIONS, -position * GraphicsConstants.VIEWPORT_DIMENSIONS);
            }
        }

        public abstract void Update(InputController inputController);
        public void Draw(Canvas canvas)
        {
            canvas.PushCamera(absoluteCamera);
            DrawSelector(canvas);
            canvas.PopCamera();
        }
        protected abstract void DrawSelector(Canvas canvas);
        public virtual void PlayAudio(AudioPlayer audioPlayer) { }

        protected string ThemeName { get { return themeName; } }
        public float FloatValue { get { return floatValue; } set { floatValue = value; } }
        public int IntValue { get { return intValue; } set { intValue = value; } }
        public bool BoolValue { get { return boolValue; } set { intValue = value ? 1 : 0; } }
    }
}
