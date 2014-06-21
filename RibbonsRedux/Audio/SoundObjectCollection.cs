using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RibbonsRedux.Content;

namespace RibbonsRedux.Audio
{
    /// <summary>
    /// A collection of sound objects.
    /// </summary>
    public class SoundObjectCollection
    {
        AssetManager assets;
        protected List<SoundObject> sounds;

        /// <summary>
        /// Creates a new sound collection.
        /// </summary>
        /// <param name="audioPlayer">The audio player associated with the game.</param>
        public SoundObjectCollection(AssetManager assets)
        {
            this.assets = assets;
            sounds = new List<SoundObject>();
        }

        /*public static SoundCollection Build(AudioPlayer audio, string moduleName)
        {
            SoundCollection s = new SoundCollection();
            string[] properties = audio.AssetDictionary.GetProperties(moduleName);
            foreach (string prop in properties)
                s.sounds.Add(new SoundObject(audio.Assets.GetSFX(audio.AssetDictionary.LookupString(moduleName, prop))));
            return s;
        }*/

        /// <summary>
        /// Adds a sound to this collection.
        /// </summary>
        /// <param name="sfxobj">The sound object to add.</param>
        public void Add(SoundObject sfxobj) { sounds.Add(sfxobj); }

        /// <summary>
        /// Adds a sound to this collection based on its asset name.
        /// </summary>
        /// <param name="assetName">The asset name of the sound object to add.</param>
        public void Add(string assetName) { sounds.Add(new SoundObject(assets.GetSFX(assetName))); }

        /// <summary>
        /// Gets the total number of sounds in this collection.
        /// </summary>
        public int Length { get { return sounds.Count; } }

        public SoundObject this[int index] { get { return sounds[index]; } }
    }
}
