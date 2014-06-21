using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using RibbonsRedux.Content;

namespace RibbonsRedux.Audio
{
    public interface IAudible
    {
        void PlaySound(AudioPlayer audioPlayer);
    }

    /// <summary>
    /// The main audio engine for the game.
    /// </summary>
    public class AudioPlayer
    {
        LinkedList<SoundEffectInstance> sfxQueue;

        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;

        Cue currentCue;
        string songName;

        int framesElapsed;

        /// <summary>
        /// Creates a new audio player.
        /// </summary>
        public AudioPlayer()
        {
            sfxQueue = new LinkedList<SoundEffectInstance>();
            framesElapsed = 0;
            try //for those who don't have music files
            {
                audioEngine = new AudioEngine("Content\\XACT\\Music.xgs");
                waveBank = new WaveBank(audioEngine, "Content\\XACT\\Wave Bank.xwb");
                soundBank = new SoundBank(audioEngine, "Content\\XACT\\Sound Bank.xsb");
            }
            catch { }
        }

        /// <summary>
        /// Allows the audio engine to process time-based events. This should be called at the end of the game's update loop.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            for (LinkedListNode<SoundEffectInstance> sfx = sfxQueue.First; sfx != null; sfx = sfx.Next)
                if (sfx.Value.State == SoundState.Stopped)
                {
                    sfx.Value.Dispose();
                    sfxQueue.Remove(sfx);
                }
            if (currentCue != null && currentCue.IsStopped)
            {
                currentCue.Dispose();
                currentCue = soundBank.GetCue(songName + "_loop");
                currentCue.Play();
            }
            framesElapsed++;
        }

        public void PlaySong(string songName)
        {
            if (this.songName != songName && currentCue != null)
            {
                currentCue.Stop(AudioStopOptions.Immediate);
                currentCue.Dispose();
                currentCue = null;
            }
            else if (this.songName == songName)
                return;
            this.songName = songName;
            try
            {
                currentCue = soundBank.GetCue(songName + "_head");
                currentCue.Play();
            }
            catch { }
        }

        public void StopSong()
        {
            if (currentCue != null && !currentCue.IsDisposed && !currentCue.IsStopped)
            {
                currentCue.Stop(AudioStopOptions.Immediate);
                currentCue.Dispose();
                currentCue = null;
            }
        }

        public void Play(SoundObject sfxobj, float volume = 1f)
        {
            if (sfxobj.Playable && sfxobj.SFX != null)
            {
                SoundEffectInstance sfxi = sfxobj.SFX.CreateInstance();
                sfxQueue.AddLast(sfxi);
                sfxi.Volume = MathHelper.Min(Math.Abs(volume), 1);
                sfxi.Play();
            }
        }

        /// <summary>
        /// Plays a sound when a certain condition is set from false to true.
        /// </summary>
        /// <param name="sfxobj">The sound object to play.</param>
        /// <param name="condition">The condition that should trigger the sound effect when set from false to true.</param>
        /// <param name="volume">The volume at which the sound is played.</param>
        public void PlayOnSetTrue(SoundObject sfxobj, bool condition, float volume = 1f)
        {
            if (condition)
            {
                Play(sfxobj, volume);
                sfxobj.Playable = false;
            }
            else
                sfxobj.Playable = true;
        }

        /// <summary>
        /// Plays a sound repeatedly when a certain condition is true.
        /// </summary>
        /// <param name="sfxobj">The sound object to play.</param>
        /// <param name="condition">The condition that should trigger the sound effect when true.</param>
        /// <param name="ticksPerPlay">The frequency at which the sound is played.</param>
        /// <param name="volume">The volume at which the sound is played.</param>
        public void RepeatOnTrue(SoundObject sfxobj, bool condition, int ticksPerPlay, float volume = 1f)
        {
            if (ticksPerPlay == 0) return;
            if (condition && framesElapsed % ticksPerPlay == 0)
            {
                Play(sfxobj, volume);
            }
        }
    }
}
