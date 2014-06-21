using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RibbonsRedux.Graphics;

namespace RibbonsRedux.Context
{
    public class ScrollingLogoBackground
    {
        Texture2D texture;
        Vector2 offset;
        Vector2 offsetIncrement;
        Color backgroundColor;

        Camera absoluteCamera;

        public ScrollingLogoBackground(Texture2D texture, Color color)
        {
            this.texture = texture;
            offsetIncrement = new Vector2(texture.Width, texture.Height);
            offsetIncrement.Normalize();
            this.backgroundColor = color;
            absoluteCamera = new Camera(GraphicsConstants.VIEWPORT_DIMENSIONS, Vector2.Zero);
        }

        public void Update()
        {
            offset += 2 * offsetIncrement;
            if (offset.X > texture.Width)
            {
                offset.X -= texture.Width;
                offset.Y -= texture.Height;
            }
        }

        public void Draw(Canvas canvas)
        {
            canvas.PushCamera(absoluteCamera);
            int x = -2 * texture.Width;
            while (x < canvas.ActiveCamera.Width / GraphicsConstants.GRAPHICS_SCALE)
            {
                int y = -2 * texture.Height;
                while (y < canvas.ActiveCamera.Height / GraphicsConstants.GRAPHICS_SCALE)
                {
                    canvas.DrawTexture(texture, backgroundColor, GraphicsConstants.GRAPHICS_SCALE * (new Vector2(x, y) + offset), 0f, 1f);
                    canvas.DrawTexture(texture, backgroundColor, GraphicsConstants.GRAPHICS_SCALE * (new Vector2(x + texture.Width, y + texture.Height) + offset), 0f, 1f);
                    y += 2 * texture.Height;
                }
                x += 2 * texture.Width;
            }
            canvas.PopCamera();
        }
    }
}
