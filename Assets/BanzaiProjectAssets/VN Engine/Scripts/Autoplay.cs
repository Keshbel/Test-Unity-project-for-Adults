using UnityEngine;
using System.Collections;


namespace VNEngine
{
    // If enabled, the story automatically progresses, sending 'button presses' after all the dialogue in a node has been printed out
    public class Autoplay : MonoBehaviour
    {
        public bool auto_playing = false;   // Are we currently autoplaying?
        public float auto_play_delay = 1.5f;   // How long we wait after the voice and text is done playing

        public bool use_player_prefs = false;   // If true, PlayerPreferences will be used to save this value, and it will automatically be loaded upon bootup

        bool button_to_be_pressed = false;
        float cur_auto_play_delay = 1.5f;
        bool isAudioMuted;
        DialogueNode cur_dialogue;

        private Sprite normal_sprite;


        void Start()
        {
            if (UIManager.ui_manager.autoplay_image != null)
                normal_sprite = UIManager.ui_manager.autoplay_image.sprite;

            // Use player preferences to store whether or not we should be autoplaying
            if (use_player_prefs)
            {
                bool saved_autoplay = System.Convert.ToBoolean(PlayerPrefs.GetInt("AutoPlay", 0));
                if (saved_autoplay)
                    ToggleAutoPlay();
            }
        }


        public void ToggleAutoPlay()
        {
            auto_playing = !auto_playing;
            Debug.Log("Auto play set: " + auto_playing);

            if (use_player_prefs)
                SaveAutoPlayStatus();

            SwapSprites();
        }


        public void SaveAutoPlayStatus()
        {
            PlayerPrefs.SetInt("AutoPlay", System.Convert.ToInt32(auto_playing));
            PlayerPrefs.Save();
        }


        public void SwapSprites()
        {
            if (UIManager.ui_manager != null && UIManager.ui_manager.autoplay_image != null)
            {
                if (UIManager.ui_manager.autoplay_image.sprite == normal_sprite)
                    UIManager.ui_manager.autoplay_image.sprite = UIManager.ui_manager.autoplay_on_image;
                else
                    UIManager.ui_manager.autoplay_image.sprite = normal_sprite;
            }
        }


        void Update()
        {
            if (auto_playing
                && VNSceneManager.current_conversation != null)
            {
                // Check if our current node is a dialogue node
                Node cur_node = VNSceneManager.current_conversation.Get_Current_Node();
                if (cur_node is DialogueNode)
                {
                    DialogueNode dialogue = (DialogueNode)cur_node;

                    if (AudioListener.volume == 0)
                    {
                        isAudioMuted = true;
                        cur_auto_play_delay = auto_play_delay + 0.5f;   // Give it an extra half second if there's no voice, to let the reader read
                    }
                    else
                    {
                        isAudioMuted = false;
                        cur_auto_play_delay = auto_play_delay;
                    }

                    if ((dialogue.done_printing)// || !UIManager.ui_manager.entire_UI_panel.activeSelf) // Done printing, or UI panel is hidden
                        && (dialogue.done_voice_clip || isAudioMuted)
                        && !button_to_be_pressed)
                    {
                        cur_dialogue = dialogue;
                        button_to_be_pressed = true;
                        // Done printing and voice, wait a second or two then press the button
                        StartCoroutine(Delay_Button_Press());
                    }
                }
            }
        }


        IEnumerator Delay_Button_Press()
        {
            yield return new WaitForSeconds(cur_auto_play_delay);

            Node cur_node = VNSceneManager.current_conversation.Get_Current_Node();
            if (cur_node is DialogueNode)
            {
                DialogueNode dialogue = (DialogueNode)cur_node;

                if (dialogue == cur_dialogue)
                {
                    button_to_be_pressed = false;
                    VNSceneManager.current_conversation.Button_Pressed();
                }
                else
                {
                    button_to_be_pressed = false;
                }
            }
            else
                button_to_be_pressed = false;
        }
    }
}