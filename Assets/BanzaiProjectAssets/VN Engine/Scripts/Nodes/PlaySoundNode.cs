using UnityEngine;
using System.Collections;

namespace VNEngine
{
    public class PlaySoundNode : Node
    {
        public AudioSource sound_to_play;

        public override void Run_Node()
        {
            sound_to_play.Play();
        }


        public override void Button_Pressed()
        {
            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}