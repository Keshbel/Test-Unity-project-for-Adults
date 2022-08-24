using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNEngine
{
    public class ChangeSideNode : Node
    {
        Dialogue_Source actor_name_from = Dialogue_Source.Text_From_Editor;
        public string actor;
        private string actual_actor_name;
        public Actor_Positions destination;


        public override void Run_Node()
        {
            actual_actor_name = VNSceneManager.GetStringFromSource(actor, actor_name_from);

            if (string.IsNullOrEmpty(actual_actor_name))
            {
                Debug.LogError("Actor " + actual_actor_name + " name is null or empty", this.gameObject);
            }
            // Check if the actor is already present
            else if (ActorManager.Is_Actor_On_Scene(actual_actor_name))
            {
                // Actor is already on the scene
                Actor actor_script = ActorManager.Get_Actor(actual_actor_name);
                actor_script.position = destination;

                ActorManager.Remove_Actor_From_Positions_Lists(actor_script);
                ActorManager.Add_Actor_To(actor_script, destination);
            }
            else
                Debug.LogError("Actor " + actual_actor_name + " is not on the scene. Use EnterActorNode to place them on the scene", this.gameObject);

            Finish_Node();
        }


        public override void Button_Pressed()
        {

        }


        public override void Finish_Node()
        {
            base.Finish_Node();
        }
    }
}