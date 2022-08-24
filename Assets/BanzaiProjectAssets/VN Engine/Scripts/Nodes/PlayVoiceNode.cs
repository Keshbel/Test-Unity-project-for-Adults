using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Plays a voice clip. Ends immediately, so as not to necessarily hold this up. Use a wait node after this to pause for the voice to finish
    public class PlayVoiceNode : Node
    {
        public AudioClip voice_clip;


        public override void Run_Node()
        {
            AudioManager.audio_manager.Play_Voice_Clip(voice_clip);
            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}