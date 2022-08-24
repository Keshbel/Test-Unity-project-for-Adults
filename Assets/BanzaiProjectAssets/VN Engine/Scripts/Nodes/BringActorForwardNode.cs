using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Brings the actor to the front of the list of actors
    public class BringActorForwardNode : Node
    {
        public Dialogue_Source actor_name_from = Dialogue_Source.Text_From_Editor;
        public string actor_name;   // Actor to bring forward
        private string actual_actor_name;

        public override void Run_Node()
        {
            actual_actor_name = VNSceneManager.GetStringFromSource(actor_name, actor_name_from);
            ActorManager.Bring_Actor_To_Front(ActorManager.Get_Actor(actual_actor_name));

            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}