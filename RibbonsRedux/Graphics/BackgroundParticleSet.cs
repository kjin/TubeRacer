using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RibbonsRedux.Content;

namespace RibbonsRedux.Graphics
{
    public class BackgroundParticleSettings
    {
        int numParticles;
        Vector2 rotationRateRange;
        Vector2 scaleRange;
        Color colorLow;
        Color colorHigh;
        Texture2D texture;
        float distance;

        public BackgroundParticleSettings(AssetManager assets, int actNumber)
        {
            TextDictionary td = new TextDictionary(assets.GetText("particles"));
            string act = "act" + actNumber;
            numParticles = td.LookupInt32(act, "numParticles");
            if (numParticles > 0)
            {
                rotationRateRange = td.LookupVector2(act, "rotationRateRange");
                scaleRange = td.LookupVector2(act, "scaleRange");
                colorLow = td.LookupColor(act, "colorLow");
                colorHigh = td.LookupColor(act, "colorHigh");
                texture = assets.GetTexture(td.LookupString(act, "texture"));
                distance = td.LookupSingle(act, "dist");
            }
        }

        public int NumParticles { get { return numParticles; } }

        private static float GetRandomValue(Vector2 range)
        {
            return MathHelper.Lerp(range.X, range.Y, (float)GraphicsHelper.Random.NextDouble());
        }

        public float RandomRotationRate() { return GetRandomValue(rotationRateRange); }
        public float RandomScale() { return GetRandomValue(scaleRange); }
        public Color RandomColor() { return Color.Lerp(colorLow, colorHigh, (float)GraphicsHelper.Random.NextDouble()); }

        public Texture2D Texture { get { return texture; } }

        public float Distance { get { return distance; } }
    }

    public class BackgroundParticle
    {
        Vector2 screenSize;

        //random parameters
        Vector2 initialPosition;
        float amplitude;
        float period;
        float speed;
        float phase;
        float rotationRate;
        float timePhase;

        float time;
        Vector2 position;
        float rotation;
        float scale;
        Color color;

        public BackgroundParticle(BackgroundParticleSettings bps, Vector2 screenSize)
        {
            Random random = GraphicsHelper.Random;
            initialPosition = new Vector2((float)random.NextDouble(), (float)random.NextDouble()) * screenSize;
            amplitude = (float)random.NextDouble() * screenSize.Y / 2;
            speed = ((float)random.NextDouble() + 1) / 100;
            period = (float)random.NextDouble() / screenSize.X * MathHelper.TwoPi;
            phase = (float)random.NextDouble() * MathHelper.TwoPi;
            timePhase = (float)random.NextDouble() * MathHelper.TwoPi;
            this.screenSize = screenSize;
            rotationRate = bps.RandomRotationRate() * (random.Next(0, 2) * 2 - 1);
            rotation = (float)random.NextDouble() * MathHelper.TwoPi;
            scale = bps.RandomScale();
            color = bps.RandomColor();
            time = 0;
        }

        public void Update(Vector2 cameraPosition, float globalTime)
        {
            time += (float)(Math.Sin(globalTime / 100 + timePhase) / 4 + 0.75f);
            position = initialPosition - cameraPosition * GraphicsConstants.PIXELS_PER_UNIT + new Vector2(speed * time, amplitude * (float)Math.Sin(period * time + phase));
            rotation = rotationRate * time;
            position.X = GraphicsHelper.Mod(position.X, screenSize.X);
            position.Y = GraphicsHelper.Mod(position.Y, screenSize.Y);
        }

        public Vector2 Position { get { return position; } }

        public float Rotation { get { return rotation; } }

        public float Scale { get { return scale; } }

        public Color Color { get { return color; } }
    }

    public class BackgroundParticleSet
    {
        Texture2D texture;
        BackgroundParticle[] particles;
        int frames;
        float distance;

        public BackgroundParticleSet(AssetManager assets, Vector2 screenSize, int actNumber)
        {
            BackgroundParticleSettings bps = new BackgroundParticleSettings(assets, actNumber);
            this.texture = bps.Texture;
            distance = bps.Distance;
            particles = new BackgroundParticle[bps.NumParticles];
            for (int i = 0; i < particles.Length; i++)
                particles[i] = new BackgroundParticle(bps, screenSize);
            frames = 0;
        }

        public void Update(Vector2 cameraPosition)
        {
            frames++;
            for (int i = 0; i < particles.Length; i++)
                particles[i].Update(cameraPosition / distance, frames);
        }

        public void Draw(Canvas canvas)
        {
            for (int i = 0; i < particles.Length; i++)
                canvas.DrawTexture(texture, particles[i].Color, particles[i].Position, particles[i].Rotation, particles[i].Scale);
        }

        public float Distance { get { return distance; } }
    }
}
