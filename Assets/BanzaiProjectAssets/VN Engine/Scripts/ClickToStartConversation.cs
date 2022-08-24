using UnityEngine;
using System.Collections;

namespace VNEngine
{
    // User clicks on this object and a conversation is started
    public class ClickToStartConversation : MonoBehaviour
    {
        public ConversationManager conversation_to_start;

        void OnMouseDown()
        {
            if (conversation_to_start != null && conversation_to_start.gameObject.activeSelf)
                StartCoroutine(Delay_Starting_Conversation());
            else
                Debug.Log("No conversation set to start!");
        }

        // Used to fix minor bug of UI appearing and it registering this same click on the UI
        IEnumerator Delay_Starting_Conversation()
        {
            yield return new WaitForSeconds(0.01f);

            conversation_to_start.Start_Conversation();
        }
    }
}