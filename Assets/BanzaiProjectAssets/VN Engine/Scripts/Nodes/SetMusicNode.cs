using UnityEngine;
using System.Collections;

namespace VNEngine
{
    // Sets the looping background music that's currently playing
    // This node must have a child object with an Audio Source so that the child
    // can be moved to be a child of MusicManager
    public class SetMusicNode : Node
    {
        public bool fadeOutPreviousMusic;
        public float fadeOutTime = 1.0f;

        public AudioClip new_music;

        public override void Run_Node()
        {
            if (new_music == null || new_music == AudioManager.audio_manager.background_music_audio_source.clip)
            {
                SaveManager.RemoveSaveFeature(AudioManager.audio_manager.background_music_audio_source.gameObject);
                Finish_Node();
                return;
            }

            SaveManager.SetSaveFeature(this, AudioManager.audio_manager.background_music_audio_source.gameObject);

            if (fadeOutPreviousMusic && AudioManager.audio_manager.background_music_audio_source.isPlaying)
            {
                // Fade out the previous background music for a smooth transition
                StartCoroutine(AudioManager.audio_manager.Fade_Out_Music(fadeOutTime));
                StartCoroutine(Wait(fadeOutTime));  // Wait, then add our background music
            }
            else
            {
                // If not fading out the previous background music, move the Child of this object that has the AudioSource
                // to be the child of MusicManager and have it play the AudioSource
                AudioManager.audio_manager.Set_Music(new_music);
                Finish_Node();
            }
        }


        // Waits a number of seconds before adding and playing the music. Allows the background music to fade out properly
        // before moving on.
        public IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            AudioManager.audio_manager.Set_Music(new_music);
            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}