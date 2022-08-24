using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Darkens the given actor
    public class DarkenActorNode : Node
    {
        Dialogue_Source actor_name_from = Dialogue_Source.Text_From_Editor;
        public string actor;
        private string actual_actor_name;

        public override void Run_Node()
        {
            actual_actor_name = VNSceneManager.GetStringFromSource(actor, actor_name_from);

            ActorManager.Get_Actor(actual_actor_name).Darken();

            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}