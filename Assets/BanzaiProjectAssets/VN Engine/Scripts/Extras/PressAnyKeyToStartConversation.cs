using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNEngine
{
    public class PressAnyKeyToStartConversation : MonoBehaviour
    {
        public ConversationManager conversation_to_start;


        void Update()
        {
            if (Input.anyKeyDown)
            {
                conversation_to_start.Start_Conversation();
                Destroy(this);
            }
        }
    }
}