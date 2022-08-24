using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

namespace VNEngine
{
    public class VNSceneManager : MonoBehaviour
    {
        public ConversationManager starting_conversation;

        public static bool verbose_debug_logs = false;  // Disable this if you want fewer messages in your console log

        public string Submit_key = "Submit";    // Change this to the name of any Key you've identified in the input manager
                                                // By default Submit is Enter or Return. Tapping these keys will make the game progress

        [HideInInspector]
        public static ConversationManager current_conversation;
        [HideInInspector]
        public static VNSceneManager scene_manager;
        [HideInInspector]
        public bool fast_forwarding = false;

        // LOCALIZATION
        public TextAsset Localized_Dialogue_CSV;    // Assign a CSV to this field if you wish to use use localization (support for multiple languages)
        [HideInInspector]
        public Dictionary<string, Dictionary<string, string>> Localized_Dialogue_Dictionaries;    // One dictionary is assigned per language
                                                                                                  // LOCALIZATION

        public bool hide_UI_at_start = false;   // If true, the UI panel is hidden. IT can be show with either a Show/Hide UI Node, or automatically when a Dialogue Node is run

        private string conversation_log;    // Log of all previous conversations
        // We can now create separate text logs. We access them by giving the string 'key' we assigned to them
        public static Dictionary<string, string> text_logs = new Dictionary<string, string>();
        public string Conversation_log
        {
            get
            { return conversation_log; }
            set
            {
                conversation_log = value;

                // Remove entire lines at a time, so the <> rich text formatting doesn't break (if a tag isn't properly closed, all tags break)
                while (conversation_log.Length > log_character_limit)
                    conversation_log = conversation_log.Substring(conversation_log.IndexOf("\n") + 1);

                UIManager.ui_manager.log_text.text = conversation_log;
                UIManager.ui_manager.scroll_log.text = conversation_log;
            }
        }
        private int log_character_limit = 5000;     // Unity's UI Text has a limit to how many characters can be displayed. If this is too high, you'll start getting errors

        public static bool Waiting_till_true = false;   // Used by WaitNodes. Game will pause until this is set to true. Can be set by code: VNSceneManager.Waiting_till_true = true;

        private float fast_forward_delay = 0.3f;     // When holding down the fast forward button or the SPACE bar, how fast should button presses be sent?

        // Time played, in seconds
        public float play_time;


        // DEFAULT VALUES.  Changes are saved when edited in-game using PlayerPrefs
        // The delay between printing the next text character. The lower, the faster the speed.
        [HideInInspector]
        public static float text_scroll_speed = 0.02f;     // How fast characters are displayed from dialogues
                                                           // How large the font of the text is
        [HideInInspector]
        public static int text_size = 18;
        // END DEFAULT VALUES

        public bool can_save = true;    // Whether or not the player can save via the Pause menu

        public bool load_user_preferences = true;   // Loads text size, font, volume settings, all from user preferences



        void Awake()
        {
            scene_manager = this;
            Time.timeScale = 1;

            if (Localized_Dialogue_CSV != null)
                Localized_Dialogue_Dictionaries = CSVReader.Generate_Localized_Dictionary(Localized_Dialogue_CSV);
            else
                Debug.Log("No Localized_Dialogue_CSV specified. Localization for DialogueNodes is not available", this.gameObject);
        }

        void Start()
        {
            // Hide UI initially?
            Show_UI(!hide_UI_at_start);

            if (load_user_preferences)
            {
                // Load text scrolling speed
                Text_Scroll_Speed_Change(PlayerPrefs.GetFloat("TextScrollSpeed", 0.02f));

                // Load the size of the dialogue text 
                Text_Size_Change(PlayerPrefs.GetInt("TextSize", 25));

                // Set the best fit auto size
                Text_Autosize_Change(System.Convert.ToBoolean(PlayerPrefs.GetInt("TextAutosize", 1)));

                // Enable and load multiple fonts
                if (UIManager.ui_manager.font_drop_down != null)
                    UIManager.ui_manager.font_drop_down.EnableFontMenu();
            }

            // Start the first conversation
            StartCoroutine(Start_Scene());

            StatsManager.Load_All_Items();

            if (!can_save && UIManager.ui_manager.save_button != null)
                UIManager.ui_manager.save_button.interactable = false;
        }

        public IEnumerator Start_Scene()
        {
            yield return null;
            //try
            //{
            if (starting_conversation != null)
            {
                starting_conversation.Start_Conversation();
            }
            else
                Debug.Log("Starting_conversation is null, please set this in the VNSceneManager", this.gameObject);
            //}
            /*catch (Exception e)
            {
                Debug.Log("No starting Conversation set. Drag in a Conversation into the SceneManager's Starting Conversation field.\n" + e.Message, gameObject);
            }*/
        }


        // Pass in the game object that contains a 'ConversationManager' script to start
        public void Start_Conversation(GameObject conversation)
        {
            conversation.GetComponent<ConversationManager>().Start_Conversation();
        }
        public void Start_Conversation(ConversationManager conversation)
        {
            conversation.Start_Conversation();
        }


        public void Add_To_Log(string heading, string text, string log_category="Default")
        {
            string msg_to_save = text;
            if (!string.IsNullOrEmpty(heading))
                msg_to_save = "<b>" + heading + "</b>: " + msg_to_save;
            msg_to_save += "\n";
            Conversation_log += msg_to_save;

            // Save the new message in 
            if (!text_logs.ContainsKey(log_category))
                text_logs.Add(log_category, "");
            text_logs[log_category] += msg_to_save;

            Canvas.ForceUpdateCanvases();
            UIManager.ui_manager.scroll_log_rect.verticalNormalizedPosition = 0;
        }

        /// </summary>
        // Calls Button_Pressed on the current conversation
        // Hierarchy of button presses: SceneManager -> Current Conversation -> Current Node
        /// </summary>
        public void Button_Pressed()
        {
            if (current_conversation != null)
                current_conversation.Button_Pressed();
        }


        // Goes to the previous node in the Conversation. Cannot go to a previous Conversation. 
        // If allowing this feature, be careful with how many Conversations you use, as you cannot go back to a previous Conversation
        public void Back_Button_Pressed()
        {
            if (current_conversation)
            {
                current_conversation.Go_Back_One_Node();
            }
        }


        float super_speed_delay = 0;


        void Update()
        {
            play_time += Time.deltaTime;    // Does not increase when paused

            super_speed_delay -= Time.deltaTime * Time.timeScale;

            // Check for user input

            // If the user is holding down any of the below buttons, make it go SUPER FAST
            if (Input.GetButtonDown(Submit_key)
                || (super_speed_delay <= 0 && (Input.GetButton(Submit_key)))   // Holding down space bar
                || (fast_forwarding && super_speed_delay <= 0)  // Holding down the 'FAST' button
                )
            {
                super_speed_delay = fast_forward_delay;
                this.Button_Pressed();
            }
        }

        // Sends a button press every fast_forward_delay seconds
        public void Fast_Forward()
        {
            fast_forwarding = true;
        }
        public void Stop_Fast_Forward()
        {
            fast_forwarding = false;
        }


        public void Show_UI(bool show_ui)
        {
            if (UIManager.ui_manager.entire_UI_panel != null)
                UIManager.ui_manager.entire_UI_panel.SetActive(show_ui);
            if (UIManager.ui_manager.ingame_UI_menu != null)
                UIManager.ui_manager.ingame_UI_menu.SetActive(show_ui);

            if (!show_ui)
                Stop_Fast_Forward();
        }


        // Returns the associated value with the given key for the language that has been set
        // Returns "" if the key is null or empty
        public string Get_Localized_Dialogue_Entry(string key)
        {
            // Check for any potential errors
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Get_Localized_Dialogue_Entry key passed in is null or empty", this.gameObject);
                return "";
            }
            if (!Localized_Dialogue_Dictionaries.ContainsKey(LocalizationManager.Language))
            {
                Debug.LogError("Get_Localized_Dialogue_Entry could not find language " + LocalizationManager.Language, this.gameObject);
                return "";
            }
            if (!Localized_Dialogue_Dictionaries[LocalizationManager.Language].ContainsKey(key))
            {
                Debug.LogError("Get_Localized_Dialogue_Entry could not find the key " + key, this.gameObject);
                return "";
            }

            return Localized_Dialogue_Dictionaries[LocalizationManager.Language][key];
        }


        // Setting the delay for printing out the next character in the dialogue window.
        public void Text_Scroll_Speed_Change(float new_speed)
        {
            if (UIManager.ui_manager.text_scroll_speed_slider != null)
                UIManager.ui_manager.text_scroll_speed_slider.value = new_speed;

            text_scroll_speed = new_speed;
            PlayerPrefs.SetFloat("TextScrollSpeed", new_speed);
            PlayerPrefs.Save();
        }
        public void Text_Autosize_Change(bool toggle)
        {
            PlayerPrefs.SetInt("TextAutosize", System.Convert.ToInt32(toggle));

            if (UIManager.ui_manager.text_autosize != null
                && UIManager.ui_manager.text_panel != null)
            {
                UIManager.ui_manager.text_autosize.isOn = toggle;
                if (toggle)
                {
                    // Turn on best fit
                    UIManager.ui_manager.text_panel.resizeTextForBestFit = true;
                    UIManager.ui_manager.text_size_slider.interactable = false;
                }
                else
                {
                    // Turn off best fit for manual sizing
                    UIManager.ui_manager.text_panel.resizeTextForBestFit = false;
                    UIManager.ui_manager.text_size_slider.interactable = true;
                }
            }
            PlayerPrefs.Save();
        }
        public void Text_Size_Change(float new_text_size)
        {
            text_size = (int)new_text_size;
            PlayerPrefs.SetInt("TextSize", text_size);
            PlayerPrefs.Save();

            if (UIManager.ui_manager.text_size_slider != null)
                UIManager.ui_manager.text_size_slider.value = text_size;

            if (UIManager.ui_manager.text_panel != null)
                UIManager.ui_manager.text_panel.fontSize = text_size;
        }



        // Returns a string given a string and a string source
        // If the source is from a string stat, and that string stat has not been set before, it returns the key initially given to it
        public static string GetStringFromSource(string incoming_value, Dialogue_Source source, bool fetch_localized_stat_value=false)
        {
            string output = incoming_value;
            switch (source)
            {
                case Dialogue_Source.Text_From_Editor:
                    output = incoming_value;
                    break;
                case Dialogue_Source.Text_From_String_Stat:
                    // Is there a string stat of this name?
                    output = StatsManager.Get_String_Stat(incoming_value);
                    if (string.IsNullOrEmpty(output))
                    {
                        output = incoming_value;
                    }
                    // If this is true, we should get the string stat, then searched through our localized UI CSV for an entry matching our string
                    else if (fetch_localized_stat_value)
                    {
                        output = UIManager.ui_manager.Get_Localized_UI_Entry(output);
                    }
                    break;
                case Dialogue_Source.Localized_Text_From_CSV:
                    output = UIManager.ui_manager.Get_Localized_UI_Entry(incoming_value);
                    break;
            }
            return output;
        }
    }
}