using UnityEngine;
using System.Collections;
using VNEngine;

namespace VNEngine
{
    public class ChangeMusic : Node
    {

        public override void Run_Node()
        {
            StartCoroutine(AudioManager.audio_manager.Fade_Out_Music(5));
            AudioManager.audio_manager.Set_Music(gameObject.GetComponent<AudioSource>().clip);
            SaveManager.SetSaveFeature(this, AudioManager.audio_manager.background_music_audio_source.gameObject);
            Finish_Node();
        }


        public override void Finish_Node()
        {
            base.Finish_Node();
        }
    }
}