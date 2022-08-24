using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Removes the actor from the scene. If sliding out, waits until the actor if offscreen first
    public class ExitActorNode : Node
    {
        public Dialogue_Source actor_name_from = Dialogue_Source.Text_From_Editor;
        public string actor_name;
        private string actual_actor_name;
        public bool slide_out = false;  // If true, have the character slide out before being destroyed
        public bool fade_out = false;
        public float fade_out_time = 1.0f;

        public override void Run_Node()
        {
            actual_actor_name = VNSceneManager.GetStringFromSource(actor_name, actor_name_from);

            if (ActorManager.Is_Actor_On_Scene(actual_actor_name))
            {
                if (!slide_out && !fade_out)
                {
                    ActorManager.Remove_Actor(actual_actor_name);
                    Finish_Node();
                }
                else
                {
                    Actor actor = ActorManager.Get_Actor(actual_actor_name);

                    if (fade_out)
                    {
                        // Fade out
                        actor.Fade_Out(fade_out_time);
                    }
                    else
                    {
                        // Slide out
                        actor.Slide_Out(1f);
                    }

                    StartCoroutine(Wait(actor));
                }
            }
            else
            {
                Debug.Log(actor_name + " is not on the scene. Remember to correctly name your actor and use 'EnterActorNode'");

                Finish_Node();
            }
        }


        // Wait until the actor has left the scene
        IEnumerator Wait(Actor actor)
        {
            while (actor != null && actor.gameObject.activeSelf)
                yield return new WaitForSeconds(0.1f);
            Finish_Node();
        }


        public override void Finish_Node()
        {
            base.Finish_Node();
        }
    }
}