using UnityEngine;
using System.Collections;

namespace VNEngine
{
    // Base class of Nodes, which make up all conversations
    // All nodes should inherit from this this class
    // If you want to write your own nodes, copy NodeTemplate.cs, and change the class name and overwrite the virtual methods you need
    public class Node : MonoBehaviour
    {
        [HideInInspector]
        public bool go_to_next_node = true;     // If true, the next node will be run during Finish_Node()
        [HideInInspector]
        public bool executed_from_load = false;

        // Called when the node is run. Override this method for inheriting classes of Node
        public virtual void Run_Node()
        {

        }


        // Called when the node is finished executing. Override this method for inheriting classes of Node.
        // Overridden methods should call base.Finish_Node() after they're done executing.
        public virtual void Finish_Node()
        {
            // Runs the next node in the conversation
            if (go_to_next_node && !executed_from_load)
                this.GetComponentInParent<ConversationManager>().Start_Next_Node();

            executed_from_load = false;
        }


        // User clicked, or hit enter. Let the node handle this in its own way. Override this method for inheriting classes of Node
        public virtual void Button_Pressed()
        {

        }


        // Stops the node in its tracks
        public virtual void Stop_Node()
        {
            this.StopAllCoroutines();
        }
    }
}