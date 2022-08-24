using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    public class Actor : MonoBehaviour
    {
        public string actor_name;       // Actor name, use this name to reference this actor in dialogue 
        public Actor_Positions position;
        public Transform custom_position;      // Use to place an actor at a very specific position
        private float brigthness_change = 0.05f;

        [HideInInspector]
        public RectTransform rect;
        bool remove_when_out_of_sight = false;  // Called when sliding the actor out


        public Vector3 desired_position = Vector3.zero;    // Smoothly move actor to this position
       
        public string default_animation_name;   // If there's an animator, play this animation when we first enter
        [HideInInspector]
        public Image fading_child_image;    // Used when switching character images
        [HideInInspector]
        public Image cur_image;
        [HideInInspector]
        public Vector3 original_scale;
        [HideInInspector]
        public bool is_moving = false;

        private Sprite default_image;    // Image the prefab is saved with


        void Awake()
        {
            rect = GetComponent<RectTransform>();
            cur_image = this.GetComponent<Image>();
            default_image = cur_image.sprite;
            original_scale = rect.localScale;

            if (this.transform.childCount > 0)
                fading_child_image = this.transform.GetChild(0).GetComponent<Image>();
        }
        void Start()
        {
            rect = GetComponent<RectTransform>();

            Play_Animation(default_animation_name);
        }

        // Makes this actor play the designated animation from its animator controller
        public void Play_Animation(string animation_name)
        {
            if (!string.IsNullOrEmpty(animation_name))
            {
                this.GetComponent<Animator>().Play(animation_name);
            }
        }



        public void Reset()
        {
            /*remove_when_out_of_sight = false;
            GetComponent<Image>().color = Color.white;
            GetComponent<Image>().sprite = default_image;
            GetComponent<Image>().overrideSprite = default_image;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),
                                                Mathf.Abs(transform.localScale.y),
                                                Mathf.Abs(transform.localScale.z));

            Play_Animation(default_animation_name);*/
        }


        // Calls a coroutine to fade in this actor over a number of seconds specified by over_time
        public void Fade_In(float over_time)
        {
            this.StopAllCoroutines();
            StartCoroutine(Fade_In_Coroutine(over_time));
        }
        IEnumerator Fade_In_Coroutine(float over_time)
        {
            Image img = this.GetComponent<Image>();
            Color tmp_color = img.color;
            tmp_color.a = 0;
            float value = 0;

            while (value < over_time)
            {
                value += Time.deltaTime;
                tmp_color.a = Mathf.Lerp(0, 1, value / over_time);
                //img.color = tmp_color;
                yield return 0;
            }
            yield break;
        }
        // Calls a coroutine to fade out this actor over a number of seconds specified by over_time.
        // Removes the actor from the scene after fading out
        public void Fade_Out(float over_time)
        {
            this.StopAllCoroutines();
            StartCoroutine(Fade_Out_Coroutine(over_time));
        }
        IEnumerator Fade_Out_Coroutine(float over_time)
        {
            if (!ActorManager.exiting_actors.Contains(this))
                ActorManager.exiting_actors.Add(this);

            Image img = this.GetComponent<Image>();
            Color tmp_color = img.color;
            tmp_color.a = 1;
            float value = 0;

            while (value < over_time)
            {
                value += Time.deltaTime;
                tmp_color.a = Mathf.Lerp(1, 0, value / over_time);
                //img.color = tmp_color;
                yield return 0;
            }

            // Remove this actor after we're done
            Debug.Log("Removing actor " + actor_name);
            ActorManager.Remove_Actor(this.actor_name);

            yield break;
        }


        // Calls a coroutine to slide in this actor over a number of seconds specified by over_time
        public void Slide_In(Actor_Positions destination, float over_time)
        {
            rect = GetComponent<RectTransform>();

            ActorManager.Add_Actor_To(this, destination);

            // Set our starting point so we slide into our desired position
            ActorManager.actor_manager.Slide_Start_Position(this, position);
        }
        // Slides the actor out, and remove the actor after it has gone
        public void Slide_Out(float over_time)
        {
            rect = GetComponent<RectTransform>();

            if (!remove_when_out_of_sight)
            {
                if (this.position == Actor_Positions.LEFT)
                {
                    ActorManager.left_actors.Remove(this);
                    desired_position = ActorManager.actor_manager.offscreen_left.localPosition;
                    if (!ActorManager.exiting_actors.Contains(this))
                        ActorManager.exiting_actors.Add(this);
                }
                else if (this.position == Actor_Positions.RIGHT)
                {
                    ActorManager.right_actors.Remove(this);
                    desired_position = ActorManager.actor_manager.offscreen_right.localPosition;
                    if (!ActorManager.exiting_actors.Contains(this))
                        ActorManager.exiting_actors.Add(this);
                }
                else if (this.position == Actor_Positions.CENTER)
                {
                    ActorManager.center_actors.Remove(this);
                    desired_position = ActorManager.actor_manager.offscreen_right.localPosition;
                    if (!ActorManager.exiting_actors.Contains(this))
                        ActorManager.exiting_actors.Add(this);
                }
                else if (this.position == Actor_Positions.CUSTOM)
                {
                    desired_position = ActorManager.Closest_Side_To_Exit(this);
                    if (!ActorManager.exiting_actors.Contains(this))
                        ActorManager.exiting_actors.Add(this);
                }
                desired_position.y = 0;// ActorManager.Get_Actor_Y_Position(this);

                remove_when_out_of_sight = true;
            }
        }

        public void Darken()
        {
            this.StopAllCoroutines();
            StartCoroutine(Darken_Coroutine());
        }
        IEnumerator Darken_Coroutine()
        {
            float value = 0;
            while (this.GetComponent<Image>().color != Color.gray)
            {
                value += brigthness_change;
                this.GetComponent<Image>().color = Color.Lerp(Color.white, Color.black, value);
                yield return new WaitForSeconds(0.03f);
            }
            yield break;
        }


        public void Lighten()
        {
            if (this.GetComponent<Image>().color != Color.white)
            {
                StopAllCoroutines();
                StartCoroutine(Lighten_Coroutine());
            }
        }
        IEnumerator Lighten_Coroutine()
        {
            float value = 0;
            while (this.GetComponent<Image>().color != Color.white)
            {
                value += brigthness_change;
                this.GetComponent<Image>().color = Color.Lerp(Color.gray, Color.white, value);
                yield return new WaitForSeconds(0.03f);
            }
            yield break;
        }


        // Instantly places the actor at the designated position
        public void Place_At_Position(Actor_Positions destination)
        {
            position = destination;

            ActorManager.Add_Actor_To(this, destination);
            this.rect = this.GetComponent<RectTransform>();
            this.rect.localPosition = this.desired_position;
        }


        public void Update()
        {
            is_moving = false;
            // Move towards desired position if we are not at our desired position
            if (this.rect.localPosition != desired_position)
            {
                this.rect.localPosition = Vector3.Lerp(this.rect.localPosition, desired_position, 3f * Time.deltaTime);
                //this.transform.position = Vector3.Lerp(this.transform.position, desired_position, 3f * Time.deltaTime);

                if (Vector2.Distance(this.rect.localPosition, desired_position) > 1f)
                    is_moving = true;
            }

            // If sliding out, check if we're visible. If we're not visible, remove the actor
            if (remove_when_out_of_sight && Mathf.Abs(this.rect.localPosition.x - desired_position.x) <= 30)
            {
                ActorManager.Remove_Actor(this.actor_name);
            }
        }
    }
}