using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Plays a sound effect
    // Can either create a prefab for the new sound effect OR use an audiosource attached to this object
    public class PlaySoundEffectNode : Node
    {
        // In the DialogueCanvas, under AudioManager that creates a new object for every new sound effect to play, each of which uses the sound effects volume slider for volume level
        public bool use_audiosource_on_this_object;
        public AudioClip sound_clip;


        public override void Run_Node()
        {
            if (sound_clip == null || !use_audiosource_on_this_object)
                gameObject.GetComponent<AudioSource>().Play();
            else
                AudioManager.audio_manager.Play_Sound_Effect(sound_clip);

            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}