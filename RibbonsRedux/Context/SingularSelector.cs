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
    /// <summary>
    /// A selector that allows users to select between several text options.
    /// </summary>
    public class SingularSelector : Selector
    {
        Option option;
        int blinkRate;

        bool selected;
        SoundObject sound;

        int timer;

        public SingularSelector(AssetManager assets, string themeName, Vector2 position, Anchor anchor, Option option)
            : base(themeName, position, 0)
        {
            TextDictionary assetDictionary = new TextDictionary(assets.GetText("selector"));
            string optionTheme = assetDictionary.LookupString(themeName, "optionTheme");
            blinkRate = assetDictionary.LookupInt32(themeName, "blinkRate");
            if (assetDictionary.CheckPropertyExists(themeName, "sound"))
                sound = new SoundObject(assets.GetSFX(assetDictionary.LookupString(themeName, "sound")));
            option.Initialize(assets, optionTheme);

            //this.position *= GraphicsConstants.VIEWPORT_DIMENSIONS / GraphicsConstants.DEFAULT_DIMENSIONS;
            this.Position -= GraphicsHelper.ComputeAnchorOrigin(anchor, option.Dimensions / GraphicsConstants.VIEWPORT_DIMENSIONS);
            this.option = option;
            IntValue = 0;
            timer = 0;
            selected = false;
        }

        public override void Update(InputController inputController)
        {
            timer++;
            if (inputController.Pause.JustPressed)
                selected = true;
            option.Update(true);
        }

        protected override void DrawSelector(Canvas canvas)
        {
            if (timer % blinkRate < blinkRate / 2)
                option.Draw(canvas, Vector2.Zero);
        }

        public override void PlayAudio(AudioPlayer audioPlayer)
        {
            if (sound != null)
                audioPlayer.PlayOnSetTrue(sound, selected);
        }
    }
}
