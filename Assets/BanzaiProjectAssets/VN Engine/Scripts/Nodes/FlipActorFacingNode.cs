using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Flips the facing of the actor. (left to right, or right to left
    public class FlipActorFacingNode : Node
    {
        Dialogue_Source actor_name_from = Dialogue_Source.Text_From_Editor;
        public string actor_name;
        private string actual_actor_name;

        public override void Run_Node()
        {
            actual_actor_name = VNSceneManager.GetStringFromSource(actor_name, actor_name_from);

            Actor actor = ActorManager.Get_Actor(actual_actor_name);
            Vector3 scale = actor.transform.localScale;
            scale.x = -scale.x;
            actor.transform.localScale = scale;

            SaveManager.SetSaveFeature(this, actor.gameObject);

            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}