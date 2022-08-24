using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNEngine
{
    // Enables an object when there dialogue is done printing.
    // Could be used for buttons that only appear when Dialogue is done printing.
    public class DisplayIfDialogueIsDonePrinting : MonoBehaviour
    {
        public GameObject object_to_activate;

        void Start()
        {
            if (object_to_activate == null)
                Debug.LogError("Please set object_to_activate", this.gameObject);
        }


        void Update()
        {
            // Check if the current node is a dialogue node, and if it's done printing
            if (object_to_activate != null &&
                VNSceneManager.current_conversation != null &&
                VNSceneManager.current_conversation.Get_Current_Node() != null &&
                VNSceneManager.current_conversation.Get_Current_Node().GetType() == typeof(DialogueNode) &&
                ((DialogueNode)VNSceneManager.current_conversation.Get_Current_Node()).done_printing)
            {
                object_to_activate.SetActive(true);
            }
            else
            {
                // Make object invisible, as there is no dialogue, or the dialogue is done printing
                object_to_activate.SetActive(false);
            }
        }
    }
}