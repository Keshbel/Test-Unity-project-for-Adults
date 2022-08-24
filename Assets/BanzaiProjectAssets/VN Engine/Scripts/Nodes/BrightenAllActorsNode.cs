using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Brightens all actors on the scene
    public class BrightenAllActorsNode : Node
    {
        public override void Run_Node()
        {
            ActorManager.Brighten_All_Actors();

            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}