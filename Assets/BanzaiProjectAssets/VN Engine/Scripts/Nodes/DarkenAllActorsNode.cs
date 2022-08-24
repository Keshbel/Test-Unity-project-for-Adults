using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Darkens all actors on the scene
    public class DarkenAllActorsNode : Node
    {
        public override void Run_Node()
        {
            ActorManager.Darken_All_Actors();

            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}