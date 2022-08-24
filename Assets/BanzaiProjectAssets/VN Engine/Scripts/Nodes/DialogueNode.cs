using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VNEngine
{
    public enum Dialogue_Source { Text_From_Editor, Localized_Text_From_CSV, Text_From_String_Stat };

    // Dialogue node displays text/plays audio as if an actor is speaking.
    // Actors must be put on the scene using an Enter actor Node
    // The UI panel is automatically made visible when this node is run
    // If there is an an AudioSource attached to this, that AudioSource will be played, and stopped when the node is finished (or audio finishes and Auto is selected)
    public class DialogueNode : Node
    {
        public Dialogue_Source actor_name_from = Dialogue_Source.Text_From_Editor;    // Does our actors name come from the editor, or a CSV?
        public string actor;    // Name of actor to use for talking
        private string actual_actor;    // Actual value depends on variable actor_name_from
        public Dialogue_Source speaker_name_from = Dialogue_Source.Text_From_Editor;    // Does our speakers name come from the editor, or a CSV?
        public string textbox_title;    // Name to be placed at top of textbox. Ex: 'Bob'
        public bool fetch_localized_stat_value = false;     // If speaker_name_from.Text_From_String_Stat, you can specify if you want to localize the string value fetched from the Stat
                                                            // You'll need to add entries in your Localized_Dialogue_CSV with the possible stat values as the keys

        private AudioSource voice_clip;   // Sound to play alongside the text. Ex: Voice saying the words in the text area
        public Dialogue_Source dialogue_from = Dialogue_Source.Text_From_Editor;    // Is the dialogue text coming from you (the user) typing into the editor, or a CSV containing multiple languages?
        public string localized_key;    // Used to read from the Localized_Dialogue_CSV in the VNSceneManager
        public string text;
        public AudioClip talking_beeps;     // A very short audio clip played every time a single character of text is printed to the text panel. Supposed to kind of sound like talking (like in Undertale)
        public bool darken_all_other_characters = true;     // Darkens all other actors on the scene
        public bool bring_speaker_to_front = true;      // Changes the ordering so this actor will be in front of others
        public bool clear_text_after = false;
        public bool log_text = true;
        public string log_category = "Default";

        [HideInInspector]
        public bool done_printing = false;
        [HideInInspector]
        public bool no_voice_playing = false;
        [HideInInspector]
        public bool done_voice_clip = false; // Set to true when the voice clip accompanying this dialogue is done playing
        private bool running = false;   // Set true when Run_Node is called

        private Stack<string> formatting_tag_stack = new Stack<string>();


        public override void Run_Node()
        {
            actual_actor = VNSceneManager.GetStringFromSource(actor, actor_name_from);

            VNSceneManager.scene_manager.Show_UI(true);  // Ensure dialogue panel is visible

            running = true;

            UIManager.ui_manager.text_panel.text = "";

            // Get localized text and sub in Stats values
            GetLocalizedText(false);

            StartCoroutine(Animate_Text(text));

            // If the actor field is filled in and the actor is present on the scene
            Actor speaker = ActorManager.Get_Actor(actual_actor);
            if (actual_actor != ""
                && speaker != null)
            {
                // Slightly darken all other actors
                if (darken_all_other_characters)
                    ActorManager.Darken_All_Actors_But(speaker);

                // Lighten this actor to show this one is talking
                speaker.Lighten();

                // Bring this actor to the front
                if (bring_speaker_to_front)
                    ActorManager.Bring_Actor_To_Front(speaker);
            }

            if (voice_clip != null && voice_clip.clip != null)
            {
                // Play audio if there is any
                voice_clip.volume = AudioManager.audio_manager.voice_volume;
                voice_clip.Play();
            }
            else
            {
                done_voice_clip = true;
                no_voice_playing = true;
            }
        }


        public override void Button_Pressed()
        {
            if (done_printing)
                Finish_Node();
            else
            {
                // Show all the text in this dialogue. This lets fast readers click to show all the dialog
                // instead of waiting it all to appear. User must click again to finish this dialogue piece.
                done_printing = true;
                StopAllCoroutines();
                UIManager.ui_manager.text_panel.text = text;
                Canvas.ForceUpdateCanvases();
            }
        }


        // Called if the node is interrupted
        public override void Stop_Node()
        {
            base.Stop_Node();

            if (voice_clip != null)
                this.voice_clip.Stop();
        }


        public override void Finish_Node()
        {
            if (voice_clip != null)
                voice_clip.Stop();

            formatting_tag_stack.Clear();
            StopAllCoroutines();

            if (log_text)
            {
                // Record what was said in the log so players can go back and read anything they missed
                VNSceneManager.scene_manager.Add_To_Log(UIManager.ui_manager.speaker_panel.text, text, log_category:this.log_category);
            }

            if (clear_text_after)
            {
                UIManager.ui_manager.speaker_panel.text = "";
                UIManager.ui_manager.text_panel.text = "";
            }

            done_printing = false;
            done_voice_clip = false;
            running = false;

            base.Finish_Node();
        }


        // Prints the text to the UI manager's dialogue text one character at a time.
        // It waits time_between_characters seconds before adding on the next character.
        public IEnumerator Animate_Text(string strComplete)
        {
            string finalized_text = "";

            // Loop through each 'word', printing 1 character at a time
            // Words are separated by spaces
            string[] words = strComplete.Split(' ');
            // This technique is done to ensure words begin printing on the lowest line (instead of printing on an upper line, not fitting and then moving down)
            for (int z = 0; z < words.Length; z++)
            {
                string cur_word = words[z];


                // NEWLINE CHECK
                // Check if this word will become too big and needs to be put on a new line
                int line_count = UIManager.ui_manager.text_panel.cachedTextGenerator.lineCount;
                // Remove any formatting tags, as they don't count for line length
                string non_formatted_word = Regex.Replace(cur_word, "<.*?>", string.Empty) + Get_All_Current_Formatting_Tags();
                if (!string.IsNullOrEmpty(finalized_text))
                {
                    finalized_text += " ";
                    UIManager.ui_manager.text_panel.text = finalized_text + non_formatted_word;
                }
                else
                    UIManager.ui_manager.text_panel.text = non_formatted_word;

                Canvas.ForceUpdateCanvases();
                int check_line_count = UIManager.ui_manager.text_panel.cachedTextGenerator.lineCount;

                if (check_line_count > line_count && line_count > 0)
                {
                    finalized_text += "\n";
                }
                UIManager.ui_manager.text_panel.text = finalized_text;
                // END NEWLINE CHECK



                // Loop through each character
                for (int i = 0; i < cur_word.Length; i++)
                {
                    if (!UIManager.ui_manager.entire_UI_panel.activeInHierarchy)
                    {
                        done_printing = true;
                    }


                    // FORMATTING TAGS https://docs.unity3d.com/Manual/StyledText.html
                    // Remove any rich text formatting from this word. Ex: <b></b>, <i></i>
                    // Check if the word has a <
                    // Run this loop for every < > bracket in our current word
                    // Formatting tags CANNOT have spaces
                    int start_formatting_index = i;// cur_word.IndexOf("<", i);
                    if (cur_word[i] == '<')
                    {
                        bool ending_tag = false;
                        // Is ending tag </
                        if (start_formatting_index < cur_word.Length && cur_word[start_formatting_index + 1] == '/')
                        {
                            ending_tag = true;
                        }

                        int end_formatting_index = cur_word.IndexOf(">", start_formatting_index);

                        // Grab our formatting tag
                        string formatting_tag = cur_word.Substring(start_formatting_index, end_formatting_index - start_formatting_index + 1);
                        if (ending_tag)
                        {
                            // Ending tag, Remove the top of our formatting tag stack
                            finalized_text += formatting_tag_stack.Pop();
                        }
                        else
                        {
                            // If beginning tag, add it to our formatting tag stack
                            finalized_text += formatting_tag;
                            // This will need to be updated if new formatting tags are created
                            if (formatting_tag.Contains("b>"))
                            {
                                formatting_tag_stack.Push("</b>");
                            }
                            else if (formatting_tag.Contains("i>"))
                            {
                                formatting_tag_stack.Push("</i>");
                            }
                            else if (formatting_tag.Contains("color"))
                            {
                                formatting_tag_stack.Push("</color>");
                            }
                            else if (formatting_tag.Contains("size"))
                            {
                                formatting_tag_stack.Push("</size>");
                            }
                        }

                        // Remove the formatting tag from our word
                        cur_word = cur_word.Remove(start_formatting_index, end_formatting_index - start_formatting_index + 1);
                        i--;
                    }
                    // END FORMATTING TAGS
                    else
                    {
                        // Simply add the current letter if it isn't a formatting tag
                        finalized_text += cur_word[i];

                        // Play a talking beep if one has been assigned
                        AudioManager.audio_manager.Play_Talking_Beep(talking_beeps);
                    }

                    UIManager.ui_manager.text_panel.text = finalized_text + Get_All_Current_Formatting_Tags();

                    if (VNSceneManager.text_scroll_speed != 0)
                        yield return new WaitForSeconds(VNSceneManager.text_scroll_speed);
                }
            }

            done_printing = true;
        }
        private string Get_All_Current_Formatting_Tags()
        {
            string tags = "";
            foreach (string s in formatting_tag_stack)
            {
                tags += s;
            }
            return tags;
        }


        void Start()
        {
            voice_clip = gameObject.GetComponent<AudioSource>();
        }
        // force_change means the language was changed, immediately stop animating the text and just set the text to done
        public void GetLocalizedText(bool force_change)
        {
            // Localize and/or insert stats into the title of the text box if needed
            if (!string.IsNullOrEmpty(textbox_title))
            {
                string speaker_title = VNSceneManager.GetStringFromSource(textbox_title, speaker_name_from, fetch_localized_stat_value:this.fetch_localized_stat_value);

                // if (speaker_name_from == Dialogue_Source.Text_From_Editor)
                //     speaker_title = textbox_title;
                // else if (speaker_name_from == Dialogue_Source.Localized_Text_From_CSV)
                // {
                //     // Get the key associated with the speaker's name
                //     speaker_title = UIManager.ui_manager.Get_Localized_UI_Entry(textbox_title);
                // }
                // else if (speaker_name_from == Dialogue_Source.Text_From_String_Stat)
                // {
                //     // Is there a string stat of this name?
                //     string stat_val = StatsManager.Get_String_Stat(textbox_title);

                //     // Check if it's valid
                //     if (string.IsNullOrEmpty(stat_val))
                //     {
                //         Debug.LogError("String stat " + textbox_title + " does not exist, please set the Stat before using it in a DialogueNode", this.gameObject);
                //         UIManager.ui_manager.speaker_panel.text = textbox_title;
                //     }
                //     else
                //     {
                //         if (fetch_localized_stat_value)
                //             stat_val = UIManager.ui_manager.Get_Localized_UI_Entry(textbox_title);
                //         speaker_title = stat_val;
                //     }
                // }

                UIManager.ui_manager.speaker_panel.text = speaker_title;
            }



            // If using text from a CSV, grab it now
            if (dialogue_from == Dialogue_Source.Localized_Text_From_CSV)
            {
                if (string.IsNullOrEmpty(localized_key))
                {
                    Debug.LogError("Localized key is null. Please enter a key", this.gameObject);
                    return;
                }

                text = VNSceneManager.scene_manager.Get_Localized_Dialogue_Entry(this.localized_key);
                if (string.IsNullOrEmpty(text))
                    Debug.LogError("Retrieved localized dialogue is null or empty. Please ensure you have entered a row with the key matching: " + this.localized_key, this.gameObject);

                // If done printing, just change the text
                if (!force_change && done_printing)
                    UIManager.ui_manager.text_panel.text = text;

                if (force_change && !done_printing)
                {
                    Button_Pressed();
                }

                // Go through text, checking to see if we need to insert Stats values
                text = Insert_Stats_into_Text(text);

                UIManager.ui_manager.text_panel.text = text;
            }
            else
            {
                // Go through text, checking to see if we need to insert Stats values
                text = Insert_Stats_into_Text(text);
            }
        }


        public string Insert_Stats_into_Text(string in_text)
        {
            // Find any [ ] characters
            string[] splits = in_text.Split('[', ']');

            // Now check each split if it's legitimate
            foreach (string original_s in splits)
            {
                bool is_variable = false;
                string new_s = "";
                string modified_s;
                if (original_s.StartsWith("b:"))
                {
                    is_variable = true;
                    modified_s = original_s.Replace("b:", "");
                    new_s = StatsManager.Get_Boolean_Stat(modified_s).ToString();
                }
                else if (original_s.StartsWith("f:"))
                {
                    is_variable = true;
                    modified_s = original_s.Replace("f:", "");
                    new_s = StatsManager.Get_Numbered_Stat(modified_s) + "";
                }
                else if (original_s.StartsWith("s:"))
                {
                    is_variable = true;
                    modified_s = original_s.Replace("s:", "");
                    new_s = StatsManager.Get_String_Stat(modified_s);
                }

                if (is_variable)
                    in_text = in_text.Replace("[" + original_s + "]", new_s);
            }

            return in_text;
        }


        void Update()
        {
            // If running and playing voice, keep checking to see if we're done our voice clip
            if (running && !done_voice_clip)
            {
                voice_clip.volume = AudioManager.audio_manager.voice_volume;    // Constantly set the volume to the correct level
                done_voice_clip = !voice_clip.isPlaying;
            }
        }
    }
}