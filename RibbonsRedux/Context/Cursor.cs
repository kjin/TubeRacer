using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RibbonsRedux.Graphics;
using RibbonsRedux.Input;
using RibbonsRedux.Audio;
using RibbonsRedux.Content;

namespace RibbonsRedux.Context
{
    public abstract class Cursor
    {
        protected List<Option> options;
        protected float smoothness;
        int cycleDelay;
        int cycleInterval;
        float spacing;

        protected SoundObject cycleSound;
        protected SoundObject selectSound;

        protected int currentValue;
        protected int previousValue;
        protected float t;
        protected bool selected;

        public bool OnTop;

        public Cursor(bool onTop = true) { t = 1f; this.OnTop = onTop; }

        public bool Cyclable(InputControl control)
        {
            return control.JustPressed || (control.ActiveTime > cycleDelay && (control.ActiveTime - cycleDelay) % cycleInterval == 0);
        }

        public virtual void Initialize(List<Option> options, AssetManager assets, string themeName)
        {
            TextDictionary assetDictionary = new TextDictionary(assets.GetText("cursor"));
            this.options = options;
            try { cycleDelay = assetDictionary.LookupInt32(themeName, "cycleDelay"); }
            catch { cycleDelay = 1; }
            try { cycleInterval = assetDictionary.LookupInt32(themeName, "cycleInterval"); }
            catch { cycleInterval = 1; }
            spacing = assetDictionary.LookupSingle(themeName, "spacing");
            if (assetDictionary.CheckPropertyExists(themeName, "cycleSound"))
                cycleSound = new SoundObject(assets.GetSFX(assetDictionary.LookupString(themeName, "cycleSound")));
            if (assetDictionary.CheckPropertyExists(themeName, "selectSound"))
                selectSound = new SoundObject(assets.GetSFX(assetDictionary.LookupString(themeName, "selectSound")));
            selected = false;
        }

        public virtual void Update(int value)
        {
            t = MathHelper.Lerp(t, 1, smoothness);
            if (value != currentValue)
            {
                t = 0;
                previousValue = currentValue;
                currentValue = value;
            }
        }

        public virtual void Draw(Canvas canvas, Vector2 selectorPosition) { }

        public void Select()
        {
            selected = true;
        }

        public virtual void PlayAudio(AudioPlayer audioPlayer)
        {
            if (cycleSound != null)
                audioPlayer.PlayOnSetTrue(cycleSound, t == 0);
            if (selectSound != null)
                audioPlayer.PlayOnSetTrue(selectSound, selected);
        }

        public float Spacing { get { return spacing; } }

        public abstract Vector2 Position { get; }
    }

    public class TextureCursor : Cursor
    {
        protected Texture2D graphic;
        protected Color color;
        protected Vector2 offset;
        protected Anchor anchor;

        protected Vector2 currentPosition;
        public override Vector2 Position { get { return currentPosition; } }

        public TextureCursor(bool onTop = true) : base(onTop) {}

        public override void Initialize(List<Option> options, AssetManager assets, string themeName)
        {
            TextDictionary assetDictionary = new TextDictionary(assets.GetText("cursor"));
            base.Initialize(options, assets, themeName);

            graphic = assets.GetTexture(assetDictionary.LookupString(themeName, "graphic"));
            try { color = new Color(assetDictionary.LookupVector4(themeName, "color")); }
            catch { color = new Color(assetDictionary.LookupVector3(themeName, "color")); }
            offset = assetDictionary.LookupVector2(themeName, "offset");
            //optional
            bool anchorParseSuccess = Enum.TryParse<Anchor>(assetDictionary.LookupString(themeName, "anchor"), out anchor);
            if (!anchorParseSuccess)
                anchor = Anchor.TopLeft;
            try { smoothness = assetDictionary.LookupSingle(themeName, "smoothness"); }
            catch { smoothness = 0f; }
        }

        public override void Update(int value)
        {
            base.Update(value);
            currentPosition = Vector2.Lerp(options[previousValue].Position, options[currentValue].Position, t) + offset;
        }

        public override void Draw(Canvas canvas, Vector2 selectorPosition)
        {
            canvas.DrawTexture(graphic, color, currentPosition + selectorPosition, anchor, 0, 0.5f);
        }
    }

    public class DoubleTextureCursor : TextureCursor
    {
        Vector2 offset2;
        Anchor anchor2;
        Vector2 currentPosition2;

        public DoubleTextureCursor(bool onTop = true) : base(onTop) {}

        public override void Initialize(List<Option> options, AssetManager assets, string themeName)
        {
            base.Initialize(options, assets, themeName);
            anchor2 = GraphicsHelper.Reverse(anchor);
            offset2 = new Vector2(-offset.X, offset.Y);
        }

        public override void Update(int value)
        {
            base.Update(value);
            currentPosition2 = Vector2.Lerp(options[previousValue].Position + new Vector2(options[previousValue].Dimensions.X, 0),
                                            options[currentValue].Position + new Vector2(options[currentValue].Dimensions.X, 0), t) + offset2;
        }

        public override void Draw(Canvas canvas, Vector2 selectorPosition)
        {
            base.Draw(canvas, selectorPosition);
            canvas.DrawTexture(graphic, color, currentPosition2 + selectorPosition, anchor2, 0, 0.5f, true);
        }
    }
}
