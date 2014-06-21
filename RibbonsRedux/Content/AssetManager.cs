using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using RibbonsRedux.Graphics;

namespace RibbonsRedux.Content
{
    /// <summary>
    /// Stores all importable content.
    /// </summary>
    public class AssetManager
    {
        class AssetCollection<T> where T : class
        {
            string directory;
            Dictionary<string, T> assets;
            List<string> assetNames;

            public AssetCollection(string directory)
            {
                this.directory = directory;
                assets = new Dictionary<string, T>();
                assetNames = new List<string>();
            }

            public void AddAsset(string identifier, T asset)
            {
                assets[identifier] = asset;
                assetNames.Add(identifier);
            }

            public T GetAsset(string identifier)
            {
                T asset;
                bool success = assets.TryGetValue(identifier, out asset);
                if (!success)
                    Console.WriteLine("Warning: {0} {1} not found. The game may crash.", typeof(T), identifier);
                return success ? asset : null;
            }

            public string Directory { get { return directory; } }

            public List<string> GetAssetNames()
            {
                return new List<string>(assetNames);
            }
        }

        AssetCollection<SoundEffect> sounds;
        AssetCollection<Song> songs;
        AssetCollection<Texture2D> textures;
        AssetCollection<Texture2D> animations;
        AssetCollection<Texture2D> masks;
        AssetCollection<Texture2D[]> miasmaAnimations;
        AssetCollection<SpriteFont> fonts;
        AssetCollection<Effect> effects;
        AssetCollection<Text> text;

        int NUM_MIASMA_FRAMES = 120;

        /// <summary>
        /// Constructs a new instance of AssetManager.
        /// </summary>
        public AssetManager()
        {
            sounds = new AssetCollection<SoundEffect>("SFX");
            songs = new AssetCollection<Song>("Music");
            textures = new AssetCollection<Texture2D>("Textures");
            animations = new AssetCollection<Texture2D>("Animations");
            masks = new AssetCollection<Texture2D>("Masks");
            miasmaAnimations = new AssetCollection<Texture2D[]>("Animations");
            effects = new AssetCollection<Effect>("Effects");
            fonts = new AssetCollection<SpriteFont>("Fonts");
            text = new AssetCollection<Text>("Text");
        }

        /// <summary>
        /// Scans the content project folder for assets, and loads them.
        /// </summary>
        /// <param name="content">The ContentManager object associated with the game.</param>
        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Console.Write("Loading all assets... ");
            DateTime before = DateTime.Now;
            try
            {
                LoadNormalContent<SoundEffect>(content, sounds);
                LoadNormalContent<Song>(content, songs);
            }
            catch
            {
                Console.WriteLine("Warning: The audio device is unavailable... no sounds will be loaded.");
            }
            LoadNormalContent<Texture2D>(content, textures);
            LoadNormalContent<Texture2D>(content, animations);
            LoadNormalContent<Texture2D>(content, masks);
            LoadNormalContent<Effect>(content, effects);
            LoadNormalContent<SpriteFont>(content, fonts);
            LoadNormalContent<Text>(content, text);

            DateTime after = DateTime.Now;
            Console.WriteLine("loaded in {0} seconds.", TimeSpan.FromTicks(after.Ticks - before.Ticks).TotalSeconds);
            //load sprites
            /*string[] spriteFiles = GetNames<Sprite>(sprites);
            foreach (string item in spriteFiles)
            {
                Texture2D texture = content.Load<Texture2D>(item);
                Sprite sprite = new Sprite(texture);
                sprites.AddAsset(item.Substring(sprites.Directory.Length + 1), sprite);
            }*/
        }

        private static string[] GetNames<T>(AssetCollection<T> collection) where T : class
        {
            string dir = ".\\Content\\";
            if (!Directory.Exists(dir + collection.Directory))
                return new string[0];
            string[] files = Directory.GetFiles(dir + collection.Directory, "*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                int hyphen = files[i].IndexOf('-');
                if (hyphen > 10) //this is the length of string .\\Content\\
                    files[i] = files[i].Substring(dir.Length, hyphen);
                else
                    files[i] = files[i].Substring(dir.Length, files[i].Length - dir.Length - 4);
                files[i] = files[i].Replace('\\', '/');
            }
            return files;
        }

        private static void LoadNormalContent<T>(ContentManager content, AssetCollection<T> collection) where T : class
        {
            string[] items = GetNames<T>(collection);
            foreach (string item in items)
                collection.AddAsset(item.Substring(collection.Directory.Length + 1), content.Load<T>(item));
        }

        /// <summary>
        /// Gets an effect based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public Effect GetEffect(string assetName) { return effects.GetAsset(assetName); }
        /// <summary>
        /// Gets a font based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public SpriteFont GetFont(string assetName) { return fonts.GetAsset(assetName); }
        /// <summary>
        /// Gets a sound effect based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public SoundEffect GetSFX(string assetName) { return sounds.GetAsset(assetName); }
        /// <summary>
        /// Gets a song based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public Song GetSong(string assetName) { return songs.GetAsset(assetName); }
        /// <summary>
        /// Gets a text object based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public Text GetText(string assetName) { return text.GetAsset(assetName); }
        /// <summary>
        /// Gets a texture based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public Texture2D GetTexture(string assetName) { return textures.GetAsset(assetName); }
    }
}
