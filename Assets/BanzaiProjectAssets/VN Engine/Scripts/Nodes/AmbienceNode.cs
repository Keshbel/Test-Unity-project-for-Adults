using UnityEngine;
using System.Collections;

namespace VNEngine
{
    // Sets the looping background music that's currently playing
    // This node must have a child object with an Audio Source so that the child
    // can be moved to be a child of MusicManager
    public class AmbienceNode : Node
    {
        public bool fade_out_previous_ambience;
        public float fadeOutTime = 1.0f;

        public AudioClip new_ambience;

        public override void Run_Node()
        {
            if (new_ambience == AudioManager.audio_manager.ambience_audio_source.clip && AudioManager.audio_manager.ambience_audio_source.volume != 0)
            {
                Debug.Log("Ambience is already playing", this.gameObject);
                Finish_Node();
                return;
            }

            SaveManager.SetSaveFeature(this, AudioManager.audio_manager.ambience_audio_source.gameObject);

            if (fade_out_previous_ambience && AudioManager.audio_manager.ambience_audio_source.isPlaying)
            {
                // Fade out the previous ambience sounds for a smooth transition
                StartCoroutine(AudioManager.audio_manager.Fade_Out_Ambience(fadeOutTime));
                StartCoroutine(Wait(fadeOutTime));  // Wait, then add our background music
            }
            else
            {
                // If not fading out the previous ambience sounds, move the Child of this object that has the AudioSource
                // to be the child of MusicManager and have it play the AudioSource
                Debug.Log("Setting ambience " + new_ambience.name);
                AudioManager.audio_manager.Set_Ambience(new_ambience);
                Finish_Node();
            }
        }


        // Waits a number of seconds before adding and playing the music. Allows the background music to fade out properly
        // before moving on.
        public IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            AudioManager.audio_manager.Set_Ambience(new_ambience);
            Finish_Node();
        }


        public override void Finish_Node()
        {
            base.Finish_Node();
        }
    }
}