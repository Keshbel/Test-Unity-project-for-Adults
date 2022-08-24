using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Removes text in the dialogue text box one character at a time, giving it an aniamted look
    // Good for credits, or strange cutscenes
    // Make sure to uncheck 'Clear text panel after' on the previous dialogue node
    public class AnimatedUndoDialogueNode : Node
    {
        public AudioClip beeps;     // Plays 1 beep each time a character is removed


        public override void Run_Node()
        {
            StartCoroutine(Remove_Text_One_Character_At_A_Time());
        }


        IEnumerator Remove_Text_One_Character_At_A_Time()
        {
            while (UIManager.ui_manager.text_panel.text.Length != 0)
            {
                // Remove the last character of text
                UIManager.ui_manager.text_panel.text = UIManager.ui_manager.text_panel.text.Remove(UIManager.ui_manager.text_panel.text.Length - 1);

                if (beeps != null)
                    AudioManager.audio_manager.Play_Talking_Beep(beeps);

                yield return new WaitForSeconds(VNSceneManager.text_scroll_speed);
            }

            Finish_Node();
            yield return null;
        }


        public override void Button_Pressed()
        {

        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}