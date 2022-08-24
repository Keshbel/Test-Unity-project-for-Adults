using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Sets the automatic feature, which simulates the user clicking after all of the dialogue has been printed out
    public class SetAutoplayNode : Node
    {
        public bool set_autoplay_to = true;


        public override void Run_Node()
        {
            Autoplay a = VNSceneManager.scene_manager.gameObject.GetComponent<Autoplay>();

            if (a != null)
            {
                a.auto_playing = set_autoplay_to;
            }
            else
                Debug.LogError("Could not find Auutoplay script. Make sure it is attached to the VNSceneManager object.", this.gameObject);

            Finish_Node();
        }
    }
}