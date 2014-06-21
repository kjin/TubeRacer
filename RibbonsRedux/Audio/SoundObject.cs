using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace RibbonsRedux.Audio
{
    /// <summary>
    /// A class encapsulating a sound effect.
    /// </summary>
    public class SoundObject
    {
        SoundEffect sfx;
        bool playable;

        /// <summary>
        /// Constructs a new sound object.
        /// </summary>
        /// <param name="sfx">The sound effect associated with this sound object.</param>
        /// <param name="playable">Whether the sound object is playable immediately after construction.</param>
        public SoundObject(SoundEffect sfx, bool playable = true)
        {
            this.sfx = sfx;
            this.playable = playable;
        }

        /// <summary>
        /// Gets the sound effect associated with this sound object.
        /// </summary>
        public SoundEffect SFX
        {
            get
            {
                return sfx;
            }
        }

        /// <summary>
        /// Gets or sets whether this sound effect will actually play when played through AudioPlayer.
        /// </summary>
        public bool Playable
        {
            get
            {
                return playable;
            }
            set
            {
                playable = value;
            }
        }
    }
}
