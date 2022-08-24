using UnityEngine;
using System.Collections;

namespace VNEngine
{
    // Destroys the object after its audio source is done playing. Requires an audio source.
    public class DestroyAfterPlayingSound : MonoBehaviour
    {
        AudioSource source;

        // Sound effects volume is decided by the sound effects volume slider in the pause menu
        public bool use_sound_effects_volume = true;


        void Start()
        {
            source = this.GetComponent<AudioSource>();
        }


        void Update()
        {
            if (!source.isPlaying)
            {
                Debug.Log("Sound effect done playing");
                Destroy(this.gameObject);
            }
            else
                source.volume = AudioManager.audio_manager.effects_volume;
        }
    }
}