using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Removes all actors from the scene. If sliding out, waits until the actor if offscreen first
    public class ExitAllActorsNode : Node
    {
        public bool slide_out = false;  // If true, have all characters slide out before being destroyed
        public bool fade_out = false;
        public bool wait_for_actors_to_exit = true;

        public override void Run_Node()
        {
            if (!slide_out && !fade_out)
            {
                for (int x = ActorManager.actors_on_scene.Count - 1; x >= 0; x--)
                {
                    Debug.Log("Exit all immediately: " + ActorManager.actors_on_scene[x].actor_name);
                    ActorManager.Remove_Actor(ActorManager.actors_on_scene[x]);
                }
            }
            else
            {
                for (int x = ActorManager.actors_on_scene.Count - 1; x >= 0; x--)
                {
                    Debug.Log("Exit all: " + ActorManager.actors_on_scene[x].actor_name);
                    Actor actor = ActorManager.actors_on_scene[x];

                    if (fade_out)
                    {
                        // Fade out
                        actor.Fade_Out(1f);
                    }
                    else
                    {
                        // Slide out
                        actor.Slide_Out(1f);
                    }
                }
            }

            if (wait_for_actors_to_exit)
                StartCoroutine(Wait());
            else
                Finish_Node();
        }

        // Wait until the actor has left the scene
        IEnumerator Wait()
        {
            while (ActorManager.actors_on_scene.Count > 0)
            {
                yield return new WaitForSeconds(0.1f);
            }
            Finish_Node();
        }

        public override void Finish_Node()
        {
            base.Finish_Node();
        }
    }
}