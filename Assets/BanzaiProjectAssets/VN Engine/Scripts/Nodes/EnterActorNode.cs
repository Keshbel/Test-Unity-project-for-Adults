using UnityEngine;
using System.Collections;

namespace VNEngine
{
    public enum Entrance_Type { Slide_In, Fade_In, None }

    public class EnterActorNode : Node
    {

        public string actor_name;
        public Dialogue_Source actor_name_from = Dialogue_Source.Text_From_Editor;    // Does our speakers name come from the editor, or a CSV?
        private string actual_actor_name;   // Value actually used within script. It's value depends on what actor_name_from is set to
        public Entrance_Type entrance_type;
        public float fade_in_time = 1.0f;   // How long it takes to fade in, if the fade_in bool is true
        public Actor_Positions destination; // Which space the will occupy
        public Transform custom_position;




        public override void Run_Node()
        {
            // Get our actor name
            actual_actor_name = VNSceneManager.GetStringFromSource(actor_name, actor_name_from);

            Actor actor_script;

            // Check if the actor is already present
            if (ActorManager.Is_Actor_On_Scene(actual_actor_name))
            {
                // Actor is already on the scene
                Debug.Log("Actor " + actual_actor_name + " already on scene");
                actor_script = ActorManager.Get_Actor(actual_actor_name).GetComponent<Actor>();
                Finish_Node();
                return;
            }
            else
            {
                // Actor is not in the scene. Instantiate it
                if (VNSceneManager.verbose_debug_logs)
                    Debug.Log("Creating new actor " + actual_actor_name);

                if (destination == Actor_Positions.CUSTOM && custom_position != null)
                    actor_script = ActorManager.Instantiate_Actor(actual_actor_name, destination, custom_position);
                else
                    actor_script = ActorManager.Instantiate_Actor(actual_actor_name, destination);
            }
            SaveManager.SetSaveFeature(this, actor_script.gameObject);

            switch (entrance_type)
            {
                case Entrance_Type.Slide_In:
                    actor_script.Slide_In(destination, 2.0f);
                    Finish_Node();
                    break;
                case Entrance_Type.Fade_In:
                    actor_script.Place_At_Position(destination);
                    actor_script.Fade_In(fade_in_time);
                    StartCoroutine(Wait(fade_in_time + 0.2f));
                    break;
                case Entrance_Type.None:
                    Finish_Node();
                    break;
            }
        }

        IEnumerator Wait(float how_long)
        {
            yield return new WaitForSeconds(how_long);
            Finish_Node();
        }

        public override void Button_Pressed()
        {
            //Finish_Node();
        }


        public override void Finish_Node()
        {
            //StopAllCoroutines();

            base.Finish_Node();
        }
    }
}