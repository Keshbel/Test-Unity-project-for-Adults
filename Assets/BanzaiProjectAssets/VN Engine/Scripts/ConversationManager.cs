using UnityEngine;
using System.Collections;

namespace VNEngine
{
    [System.Serializable]
    public class ConversationManager : MonoBehaviour
    {
        public bool destroy_when_finished = false;
        public ConversationManager start_conversation_when_done;     // Automatically starts this conversation when finished

        // Nodes are the pieces of this conversation. We will run them in the order that the children of this gameobject is placed
        private Node[] all_nodes;
        [HideInInspector]
        public int cur_node = 0;   // Which node we are currently processing
        [HideInInspector]
        public bool active = false; // Set to true when this is current conversation
        [HideInInspector]
        public bool finished_conversation = false;


        void Start()
        {
            all_nodes = this.GetComponentsInChildren<Node>();
        }


        // Simply start the first node to get this conversation going
        public void Start_Conversation()
        {
            // Reset the previous conversation if there is one
            if (VNSceneManager.current_conversation != null && VNSceneManager.current_conversation.active)
            {
                VNSceneManager.current_conversation.Reset_Conversation();
            }

            // Set this conversation as active in the scene manager
            VNSceneManager.current_conversation = this;

            cur_node = 0;
            active = true;

            if (all_nodes.Length == 0)
                Finish_Conversation();
            else
                Start_Node();   // Start the first conversation node
        }


        // Call to start a conversation part way through
        public void Start_Conversation_Partway_Through(int node_to_start)
        {
            VNSceneManager.current_conversation = this;
            cur_node = node_to_start;
            active = true;

            if (all_nodes.Length == 0)
                Finish_Conversation();
            else
                Start_Node();
        }


        // Move onto the next node.
        // If there is no next node, this conversation is over and we should move on to the next one or load a different scene.
        public void Start_Next_Node()
        {
            cur_node++;

            if (cur_node < all_nodes.Length)
                Start_Node();
            else
            {
                Finish_Conversation();
            }
        }
        // Runs the current node
        public void Start_Node()
        {
            if (cur_node < all_nodes.Length)
                all_nodes[cur_node].Run_Node();
        }
        public void Go_Back_One_Node()
        {
            if (active && cur_node > 0)
            {
                UIManager.ui_manager.text_panel.text = "";
                UIManager.ui_manager.speaker_panel.text = "";
                all_nodes[cur_node].Stop_Node();
                cur_node--;
                Start_Node();
            }
        }


        public void Reset_Conversation()
        {
            // Stop all coroutines so they don't interfere with other nodes
            foreach (Node node in all_nodes)
            {
                node.Stop_Node();
            }

            active = false;
            cur_node = 0;
        }


        // Returns the current node in the conversation
        public Node Get_Current_Node()
        {
            return all_nodes[cur_node];
        }


        // Starts at the given node, and starts Conversation at the beginning of the node is not found
        public void Start_At_Node(Node node)
        {
            bool found_node = false;
            int x = 0;
            foreach (Node n in all_nodes)
            {
                if (n == node)
                {
                    found_node = true;
                    break;
                }
                x++;
            }

            if (found_node)
            {
                this.Start_Conversation_Partway_Through(x);
            }
            else
            {
                Debug.LogError("Node " + node.name + " not found in Start_At_Node. Starting conversation at beginning", gameObject);
                this.Start_Conversation();
            }
        }


        // Destroys this game object.
        // Be sure to have added a StartConversationNode or LoadSceneNode before the conversation is over,
        // else nothing will happen!
        public void Finish_Conversation()
        {
            Reset_Conversation();

            finished_conversation = true;

            if (destroy_when_finished)
                Destroy(this.gameObject);

            if (start_conversation_when_done)
                start_conversation_when_done.Start_Conversation();
        }


        // User clicked or hit enter. Proceed with conversation if possible
        public void Button_Pressed()
        {
            if (active)
            {
                if (cur_node < all_nodes.Length)
                    all_nodes[cur_node].Button_Pressed();
            }
        }


        void Update()
        {

        }
    }
}