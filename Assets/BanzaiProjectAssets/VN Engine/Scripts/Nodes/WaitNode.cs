using UnityEngine;
using System.Collections;

namespace VNEngine
{
    // This can be edited to your purposes, to wait for some event of your own
    public enum Wait_Until { Time, Bool_is_True };

    // Can be set to wait for a certain amount of time, or until a bool is set to true.
    // The bool can be set in code by calling (c#): 
    // VNSceneManager.VNSceneManager.Waiting_till_true = true;
    // Use this to wait for events to happen
    public class WaitNode : Node
    {
        public Wait_Until waiting_for = Wait_Until.Time;
        public float wait_for_seconds = 1.0f;


        public override void Run_Node()
        {
            VNSceneManager.Waiting_till_true = false;

            switch (waiting_for)
            {
                case Wait_Until.Time:
                    StartCoroutine(Wait(wait_for_seconds));
                    break;
                case Wait_Until.Bool_is_True:
                    StartCoroutine(Wait_Until_Bool());
                    break;
            }
        }


        public override void Finish_Node()
        {
            VNSceneManager.Waiting_till_true = false;
            base.Finish_Node();
        }


        public IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Finish_Node();
        }

        // Waits until the boolean is set to true before finishing
        public IEnumerator Wait_Until_Bool()
        {
            // Checks every frame if the bool is true
            while (!VNSceneManager.Waiting_till_true)
            {
                yield return null;
            }

            // Bool is true. The wait is over
            Finish_Node();
        }
    }
}