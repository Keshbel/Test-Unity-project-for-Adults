using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Changes the sprite being displayed for this actor
    public class ChangeActorImageNode : Node
    {
        public Dialogue_Source actor_name_from = Dialogue_Source.Text_From_Editor;
        public string actor_name;   // Actor who is to change images
        private string actual_actor_name;
        public Sprite new_image;
        public bool fade_in_new_image = true;  // Whether or not to fade out the old image
        float fade_out_time = 0.4f;     // In seconds
        public bool lighten_actor;
        public bool bring_actor_to_front;

        public override void Run_Node()
        {
            actual_actor_name = VNSceneManager.GetStringFromSource(actor_name, actor_name_from);

            if (ActorManager.Is_Actor_On_Scene(actual_actor_name))
            {
                Actor actor = ActorManager.Get_Actor(actual_actor_name);
                SaveManager.SetSaveFeature(this, actor.gameObject);

                Debug.Log("Actor on scene " + actor.actor_name);

                if (fade_in_new_image)
                {
                    Sprite old_sprite = actor.cur_image.overrideSprite;

                    if (lighten_actor)
                        actor.Lighten();
                    if (bring_actor_to_front)
                        ActorManager.Bring_Actor_To_Front(actor);

                    // Fade out old image
                    actor.fading_child_image.gameObject.SetActive(true);
                    actor.fading_child_image.overrideSprite = old_sprite;

                    // Set colour of old image to match current image
                    actor.fading_child_image.color = actor.cur_image.color;
                    actor.fading_child_image.GetComponent<RectTransform>().sizeDelta = actor.rect.sizeDelta;
                    StartCoroutine(Fade_Out_Coroutine(fade_out_time, actor.fading_child_image));

                    // Fade in new image
                    actor.cur_image.overrideSprite = new_image;
                    StartCoroutine(Fade_In_Coroutine(fade_out_time, actor.cur_image));

                    // Wait before we allow it to finish
                    StartCoroutine(Wait(actor, fade_out_time * 1.1f));
                }
                else
                {
                    Debug.Log("Actor on scene " + actor.actor_name);

                    actor.cur_image.overrideSprite = new_image;
                    Finish_Node();
                }
            }
            else
            {
                Finish_Node();

                Debug.Log(actual_actor_name + " is not on the scene. Remember to correctly name your actor and use 'EnterActorNode'");
            }
        }


        IEnumerator Wait(Actor actor, float how_long)
        {
            yield return new WaitForSeconds(how_long);
            actor.fading_child_image.gameObject.SetActive(false);
            Finish_Node();
        }
        IEnumerator Fade_In_Coroutine(float over_time, Image image)
        {
            // Set it to completely transparent
            image.color = new Color(
                image.color.r,
                image.color.g,
                image.color.b,
                0);
            float cur_time = 0;

            // Incrementally make it less transparent
            while (image.color.a != 1)
            {
                image.color = new Color(
                    image.color.r,
                    image.color.g,
                    image.color.b,
                    Mathf.Lerp(0, 1, cur_time / over_time));
                cur_time += Time.deltaTime;
                yield return 0;
            }
            yield break;
        }
        IEnumerator Fade_Out_Coroutine(float over_time, Image image)
        {
            // Set it to completely opaque
            image.color = new Color(
                image.color.r,
                image.color.g,
                image.color.b,
                1);
            float cur_time = 0;

            // Incrementally make it less transparent
            while (image.color.a != 0)
            {
                image.color = new Color(
                    image.color.r,
                    image.color.g,
                    image.color.b,
                    Mathf.Lerp(1, 0, cur_time / over_time));
                cur_time += Time.deltaTime;
                yield return 0;
            }
            yield break;
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}
