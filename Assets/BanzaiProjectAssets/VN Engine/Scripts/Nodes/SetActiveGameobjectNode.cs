using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Sets a gameobject on/off depending on the set_active bool.
    public class SetActiveGameobjectNode : Node
    {
        public GameObject object_to_modify;
        public bool set_active = true;

        public override void Run_Node()
        {
            object_to_modify.SetActive(set_active);
            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}