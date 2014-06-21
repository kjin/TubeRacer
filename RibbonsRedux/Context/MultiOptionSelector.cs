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
    /// A selector that allows users to select between several options.
    /// </summary>
    public class MultiOptionSelector : Selector
    {
        Cursor cursor;
        List<Option> options;
        MultiOptionArrangement arrangement;
        int x;
        int y;

        Vector2 offset;

        bool cursorBoundsEnabled;
        RectangleF cursorBounds;

        public MultiOptionSelector(AssetManager assets, string themeName, Vector2 position, Anchor anchor, List<Option> options, MultiOptionArrangement arrangement, Cursor cursor, int initialValue)
            : base(themeName, position, initialValue)
        {
            //process arrangement
            if (arrangement == MultiOptionArrangement.ListX)
                arrangement = new MultiOptionArrangement(options.Count, 1);
            if (arrangement == MultiOptionArrangement.ListY)
                arrangement = new MultiOptionArrangement(1, options.Count);
            this.arrangement = arrangement;

            TextDictionary assetDictionary = new TextDictionary(assets.GetText("selector"));
            //load themes
            string cursorTheme = assetDictionary.LookupString(themeName, "cursorTheme");
            string optionTheme = assetDictionary.LookupString(themeName, "optionTheme");
            Anchor justify = Anchor.Center;
            bool justifySuccess = assetDictionary.CheckPropertyExists(themeName, "justify");
            if (justifySuccess)
            {
                string justifyString = assetDictionary.LookupString(themeName, "justify");
                if (justifyString == "Left")
                    justify = Anchor.CenterLeft;
                else if (justifyString == "Right")
                    justify = Anchor.CenterRight;
            }
            //position components
            cursor.Initialize(options, assets, cursorTheme);
            Vector2 individualSize = Vector2.Zero;
            for (int i = 0; i < options.Count; i++)
            {
                options[i].Initialize(assets, optionTheme);
                if (options[i].Dimensions.X > individualSize.X)
                    individualSize.X = options[i].Dimensions.X;
                if (options[i].Dimensions.Y > individualSize.Y)
                    individualSize.Y = options[i].Dimensions.Y;
            }
            for (int i = 0; i < options.Count; i++)
                options[i].Position = (individualSize + cursor.Spacing * Vector2.One) * arrangement.GetPosition(i);
            Vector2 overallSize = new Vector2(arrangement.Columns * (individualSize.X + cursor.Spacing) - cursor.Spacing, arrangement.Rows * (individualSize.Y + cursor.Spacing) - cursor.Spacing);
            for (int i = 0; i < options.Count; i++)
            {
                Vector2 p = options[i].Position;
                if (justify == Anchor.TopCenter || justify == Anchor.Center || justify == Anchor.BottomCenter)
                    p.X += (individualSize.X - options[i].Dimensions.X) / 2;
                else if (justify == Anchor.TopRight || justify == Anchor.CenterRight || justify == Anchor.BottomRight)
                    p.X += individualSize.X - options[i].Dimensions.X;
                if (justify == Anchor.CenterLeft || justify == Anchor.Center || justify == Anchor.CenterRight)
                    p.Y += (individualSize.Y - options[i].Dimensions.Y) / 2;
                else if (justify == Anchor.BottomLeft || justify == Anchor.BottomCenter || justify == Anchor.BottomRight)
                    p.Y += individualSize.Y - options[i].Dimensions.Y;
                options[i].Position = p;
            }
            this.Position -= GraphicsHelper.ComputeAnchorOrigin(anchor, overallSize / GraphicsConstants.VIEWPORT_DIMENSIONS);
            this.options = options;
            this.cursor = cursor;
            //initialize position
            Vector2 initialPosition = arrangement.GetPosition(IntValue);
            x = (int)initialPosition.X;
            y = (int)initialPosition.Y;
            cursor.Update(IntValue);
        }

        public void EnableCursorBounds(RectangleF bounds)
        {
            cursorBoundsEnabled = true;
            this.cursorBounds = bounds;
        }

        public void DisableCursorBounds()
        {
            cursorBoundsEnabled = false;
        }

        public override void Update(InputController inputController)
        {
            //process input
            ProcessInput(inputController);
            //update options and cursor
            IntValue = arrangement.GetIndex(x, y);
            for (int i = 0; i < options.Count; i++)
                options[i].Update(i == IntValue);
            cursor.Update(IntValue);
            if (cursorBoundsEnabled)// && !cursorBounds.Contains(cursor.Position))
            {
                //offset = -cursorBounds.EdgeDistance(cursor.Position);
                cursorBounds = cursorBounds.Offset(cursorBounds.EdgeDistance(cursor.Position));
            }
        }

        protected override void DrawSelector(Canvas canvas)
        {
            if (!cursor.OnTop)
                cursor.Draw(canvas, -cursorBounds.TopLeft);
            for (int i = 0; i < options.Count; i++)
                options[i].Draw(canvas, -cursorBounds.TopLeft);
            if (cursor.OnTop)
                cursor.Draw(canvas, -cursorBounds.TopLeft);
        }

        public override void PlayAudio(AudioPlayer audioPlayer)
        {
            cursor.PlayAudio(audioPlayer);
        }

        public Vector2 CursorPosition { get { return cursor.Position; } }

        #region helper
        private void ProcessInput(InputController inputController)
        {
            if (cursor.Cyclable(inputController.MenuLeft))
            {
                x--;
                if (x < 0)
                {
                    x = arrangement.Columns - 1;
                    while (arrangement.GetIndex(x, y) >= options.Count)
                        x--;
                }
            }
            if (cursor.Cyclable(inputController.MenuRight))
            {
                x++;
                if (x >= arrangement.Columns || arrangement.GetIndex(x, y) >= options.Count)
                    x = 0;
            }
            if (cursor.Cyclable(inputController.MenuUp))
            {
                y--;
                if (y < 0)
                {
                    y = arrangement.Rows - 1;
                    while (arrangement.GetIndex(x, y) >= options.Count)
                        y--;
                }
            }
            if (cursor.Cyclable(inputController.MenuDown))
            {
                y++;
                if (y >= arrangement.Rows || arrangement.GetIndex(x, y) >= options.Count)
                    y = 0;
            }
            if (inputController.MenuForward.JustPressed)
                cursor.Select();
        }
        #endregion
    }
}
