using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace VNEngine
{
    // Used to change the current Audio Snapshot. Very snazzy to have different rooms/locations make audio sound different in different locationns.
    // https://docs.unity3d.com/ScriptReference/Audio.AudioMixer.html
    public class ChangeAudioSnapshotNode : Node
    {
        public AudioMixerSnapshot toAudioSnapshot;
        public float transitionTime;

        public override void Run_Node()
        {
            this.toAudioSnapshot.TransitionTo(this.transitionTime);

            Finish_Node();
        }


        public override void Finish_Node()
        {
            base.Finish_Node();
        }
    }
}