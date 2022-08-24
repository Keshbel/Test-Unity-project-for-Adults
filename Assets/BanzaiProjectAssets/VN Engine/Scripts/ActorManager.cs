using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace VNEngine
{
    // Actors positied on either the left or right hand side of the screen 
    // Maximum 6 actors, 3 per side, using a 'stacked card' animations
    public enum Actor_Positions { LEFT, RIGHT, CENTER, CUSTOM };

    public class ActorManager : MonoBehaviour
    {
        public static float space_per_actor_in_CENTER = 100f;   // Only affects Actors if they are in the CENTER position. Alter this to increase/decrease room the CENTER actors get

        // If true, Actors are disabled instead of being deleted when they Exit. This is more efficient, as instantiating gameObjects is resource intensive, and it's best to reuse objects as much as possible.
        // If false, Actors are deleted when they exit, and recreated if they Enter again
        public static bool use_inactive_actors = true;

        public static ActorManager actor_manager;

        public static List<Actor> actors_on_scene = new List<Actor>();      // List of all instantiated actors
        public static List<Actor> exiting_actors = new List<Actor>();   // Used so we don't switch conversations before actors have exited

        [HideInInspector]
        public static List<Actor> left_actors = new List<Actor>();     // Can have multiple actors on the left side. 0 is leftmost, 1 is next, 2 is next, etc.
                                                                       // Looks like: 0 1 2
        [HideInInspector]
        public static List<Actor> right_actors = new List<Actor>();     // 0 is rightmost, 1 is next
                                                                        // Looks like: 2 1 0
        [HideInInspector]
        public static List<Actor> center_actors = new List<Actor>();


        // Used for sliding actors in
        public RectTransform offscreen_left;
        public RectTransform offscreen_right;


        static float canvas_width;
        static float side_width;



        void Awake()
        {
            actor_manager = this;

            actors_on_scene = new List<Actor>();
            exiting_actors = new List<Actor>();
            left_actors = new List<Actor>();
            right_actors = new List<Actor>();
        }
        void Start()
        {
            // Check canvas size
            side_width = UIManager.ui_manager.canvas_width / 2;
        }


        public static bool Is_Actor_On_Scene(string actor_name)
        {
            // Search for our actor on the scene
            Actor item = actors_on_scene.Find(obj => obj.actor_name == actor_name);

            return (item != null);
        }


        // Finds the actor if it on the scene.
        // Returns null if the actor is not present
        public static Actor Get_Actor(string actor_name)
        {
            // Search for our actor on the scene
            Actor item = actors_on_scene.Find(obj => obj.actor_name == actor_name);
            return item;
        }


        private static bool Name_Match(string actor_name, Actor current_actor)
        {
            return (current_actor != null && actor_name == current_actor.name);
        }


        // Instantiates an actor from the Resources/Actors folder with the name actor_name.
        // It then sets the object as a child of the Actors object in the canvas
        public static Actor Instantiate_Actor(string actor_name, Actor_Positions destination, Transform custom_position = null)
        {
            // Check to see if the actor is already on the scene
            Actor a = ActorManager.Get_Actor(actor_name);
            if (a != null)
            {
                Debug.Log("Actor " + actor_name + " already on scene");
                return a;
            }
            if (UIManager.ui_manager.actor_parent == null)
            {
                Debug.LogError("Unable to instantiate Actor " + actor_name + " because UIManager's actor_parent field is empty. Please set actor_parent field to an object.");
            }


            // Check if there is a disabled actor available to use
            if (use_inactive_actors)
            {
                Actor[] disabled_actors = UIManager.ui_manager.actor_parent.gameObject.GetComponentsInChildren<Actor>(true);
                foreach (Actor acto in disabled_actors)
                {
                    if (acto.actor_name == actor_name)
                    {
                        // Found a correct actor, set it up correctly
                        a = acto;
                        a.Reset();
                        a.gameObject.SetActive(true);
                    }
                }
            }


            if (a == null)
            {
                // Proceed with creating a gameobject
                GameObject actor = Instantiate(Resources.Load("Actors/" + actor_name, typeof(GameObject)), UIManager.ui_manager.actor_parent) as GameObject;
                a = actor.GetComponent<Actor>();
            }

            actors_on_scene.Add(a);  // Add to list of actors

            // Place actor at custom position
            if (destination == Actor_Positions.CUSTOM && custom_position != null)
            {
                //a.rect.localPosition = custom_position.position;
                a.desired_position = new Vector3(custom_position.GetComponent<RectTransform>().localPosition.x,
                                                custom_position.GetComponent<RectTransform>().localPosition.y * a.transform.localScale.y);
                a.custom_position = custom_position;
            }
            // Use standard automatic position calculations
            else
                ActorManager.Add_Actor_To(a, destination);

            return a;
        }


        // Returns the OFFSCREEN_LEFT or OFFSCREEN_RIGHT position based on which exit the actor's closest to
        public static Vector3 Closest_Side_To_Exit(Actor actor)
        {
            if (actor.transform.localPosition.x <= 0)
                return ActorManager.actor_manager.offscreen_left.localPosition;
            else
                return ActorManager.actor_manager.offscreen_right.localPosition;
        }


        // Makes all actors slightly dark
        public static void Darken_All_Actors()
        {
            foreach (Actor actor in actors_on_scene)
            {
                actor.Darken();
            }
        }
        // Makes all actors bright (normal colour)
        public static void Brighten_All_Actors()
        {
            foreach (Actor actor in actors_on_scene)
            {
                actor.Lighten();
            }
        }


        // Called by dialogue node to darken all actors but the one that's talking
        public static void Darken_All_Actors_But(Actor speaking_actor)
        {
            foreach (Actor actor in actors_on_scene)
            {
                // Don't darken this actor
                if (actor.name != speaking_actor.name)
                {
                    actor.Darken();
                }
            }
        }


        // Brings the given actor to the front
        public static void Bring_Actor_To_Front(Actor actor)
        {
            actor.transform.SetAsLastSibling();
        }
        // Brings the given actor to the back
        public static void Bring_Actor_To_Back(Actor actor)
        {
            actor.transform.SetAsFirstSibling();
        }


        // Only removes from the sides list, not other lists. Used by ChangeSideNode
        public static void Remove_Actor_From_Positions_Lists(Actor actor)
        {
            left_actors.Remove(actor);
            right_actors.Remove(actor);
            center_actors.Remove(actor);
        }


        public static void Add_Actor_To(Actor actor, Actor_Positions position)
        {
            actor.position = position;
            List<Actor> side_list = null;

            switch (position)
            {
                case (Actor_Positions.LEFT):
                    side_list = left_actors;
                    break;
                case (Actor_Positions.RIGHT):
                    side_list = right_actors;
                    break;
                case (Actor_Positions.CENTER):
                    side_list = center_actors;
                    break;
                case (Actor_Positions.CUSTOM):
                    // Custom positions don't have a side list
                    return;
            }

            // If actor is already present on that side, do nothing
            if (side_list.Contains(actor))
            {
                return;
            }

            // Actors are added starting from the edge, working their way in
            side_list.Add(actor);

            // Set their position on the screen
            Reevaluate_All_Actor_Positions();
        }


        // Moves the actor to the inwards most position of the side they're currently on
        public static void Move_Actor_Inwards(Actor actor)
        {
            List<Actor> side_list = null;

            switch (actor.position)
            {
                case (Actor_Positions.LEFT):
                    side_list = left_actors;
                    break;
                case (Actor_Positions.RIGHT):
                    side_list = right_actors;
                    break;
                case (Actor_Positions.CENTER):
                case (Actor_Positions.CUSTOM):
                    return;
            }

            // Actors are added starting from the edge, working their way in
            side_list.Remove(actor);
            side_list.Add(actor);

            Reevaluate_All_Actor_Positions();
        }
        // Moves the actor to the outermost position of the side they're currently on
        public static void Move_Actor_Outwards(Actor actor)
        {
            List<Actor> side_list = null;

            switch (actor.position)
            {
                case (Actor_Positions.LEFT):
                    side_list = left_actors;
                    break;
                case (Actor_Positions.RIGHT):
                    side_list = right_actors;
                    break;
                case (Actor_Positions.CENTER):
                case (Actor_Positions.CUSTOM):
                    return;
            }

            // Actors are added starting from the edge, working their way in
            side_list.Remove(actor);
            side_list.Insert(0, actor);

            Reevaluate_All_Actor_Positions();
        }


        // If the order of the SIDES list has changed, call this to shuffle the actors to their new places
        public static void Reevaluate_All_Actor_Positions()
        {
            float room_to_work_with = 0;    // Each actor gets this much room to work with
            float offset_sign = -1;
            float space_taken_up_in_center = 0;
            float space_taken_up_per_side_from_center = 0;
            List<Actor> side_list = null;

            // CENTER
            // Determine how much room is left if we have actors in the center
            if (center_actors.Count > 0)
            {
                space_taken_up_in_center = center_actors.Count * space_per_actor_in_CENTER;
                space_taken_up_per_side_from_center = space_taken_up_in_center / 1.5f;
                side_list = center_actors;
                Set_Side_Positions(side_list, space_per_actor_in_CENTER, -space_taken_up_in_center / 2);
            }


            // LEFT SIDE
            offset_sign = -1;    // -1, or 1: left or right
            side_list = left_actors;
            // Each actor gets this much room to work with
            room_to_work_with = offset_sign * (side_width - space_taken_up_in_center) / side_list.Count;
            Set_Side_Positions(side_list, room_to_work_with, offset_sign * space_taken_up_per_side_from_center);


            // RIGHT SIDE
            offset_sign = 1;
            side_list = right_actors;
            // Each actor gets this much room to work with
            room_to_work_with = offset_sign * (side_width - space_taken_up_in_center) / side_list.Count;
            Set_Side_Positions(side_list, room_to_work_with, offset_sign * space_taken_up_per_side_from_center);
        }
        private static void Set_Side_Positions(List<Actor> actors_to_set, float room_to_work_with, float offset)
        {
            foreach (Actor actor in actors_to_set)
            {
                if (!exiting_actors.Contains(actor))
                {
                    // Change the index order so new characters are more center
                    float actual_index = Mathf.Clamp(actors_to_set.Count - (actors_to_set.IndexOf(actor) + 1), 0, actors_to_set.Count - 1) + 1;
                    // Calculate where THIS specific actor should be
                    float actual_position = offset + room_to_work_with * (actual_index) - (room_to_work_with / 2);
                    actor.desired_position = new Vector3(actual_position,
                                                            0, //Get_Actor_Y_Position(actor),
                                                            0);
                }
            }
        }


        public void Slide_Start_Position(Actor actor, Actor_Positions position)
        {
            RectTransform rect = actor.GetComponent<RectTransform>();

            if (position == Actor_Positions.LEFT)
                rect.localPosition = offscreen_left.localPosition;
            else if (position == Actor_Positions.RIGHT)
                rect.localPosition = offscreen_right.localPosition;
            else if (position == Actor_Positions.CENTER || position == Actor_Positions.CUSTOM)
                rect.localPosition = offscreen_right.localPosition;

            rect.localPosition = new Vector3(rect.localPosition.x,
                0, //ActorManager.Get_Actor_Y_Position(actor), 
                rect.localPosition.z);
        }


        // Remove the actor from the scene, and destroys the game object
        public static void Remove_Actor(string actor_name)
        {
            Remove_Actor(Get_Actor(actor_name));
        }
        public static void Remove_Actor(Actor actor)
        {
            if (actor == null)
                return; // Didn't find an actor matching that name

            // If the actor has an animator, reset it to the default
            Animator anim = actor.gameObject.GetComponent<Animator>();
            if (anim != null)
                anim.Rebind();


            actors_on_scene.Remove(actor);
            left_actors.Remove(actor);
            right_actors.Remove(actor);
            exiting_actors.Remove(actor);
            actor.StopAllCoroutines();

            if (use_inactive_actors)
                actor.gameObject.SetActive(false);
            else
                Destroy(actor.gameObject);

            Reevaluate_All_Actor_Positions();
        }


        public void Print_Out_Actors_On_Sides()
        {
            print("Actors on left side");
            foreach (Actor a in ActorManager.left_actors)
            {
                print(a.actor_name);
            }
            print("Actors in center");
            foreach (Actor a in ActorManager.center_actors)
            {
                print(a.actor_name);
            }
            print("Actors on right side");
            foreach (Actor a in ActorManager.right_actors)
            {
                print(a.actor_name);
            }
        }
    }
}