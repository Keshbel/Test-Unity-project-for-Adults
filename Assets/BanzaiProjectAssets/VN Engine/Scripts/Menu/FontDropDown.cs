using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace VNEngine
{
    // http://forum.unity3d.com/threads/ui-dropdown-for-fonts.385453/
    public class FontDropDown : MonoBehaviour
    {
        public Dropdown Maindropdown;
        public Text text;
        // public Font[] fonts;

        bool started = false;

        void Awake()
        {
            //EnableFontMenu();
        }
        void Start()
        {

        }

        public void EnableFontMenu()
        {
            // Clear/remove all option item
            Maindropdown.options.Clear();

            // Fill the dropdown menu OptionData with all Font Names in fonts[]
            foreach (Font f in UIManager.ui_manager.fonts)
            {
                Maindropdown.options.Add(new Dropdown.OptionData() { text = f.name });
            }

            // Load font player preference
            string saved_font = PlayerPrefs.GetString("Font", "");
            if (!string.IsNullOrEmpty(saved_font))
            {
                for (int x = 0; x < UIManager.ui_manager.fonts.Length; x++)
                {
                    if (UIManager.ui_manager.fonts[x].name == saved_font)
                    {
                        text.text = UIManager.ui_manager.fonts[x].name;
                        text.font = UIManager.ui_manager.fonts[x];
                        Maindropdown.value = x;
                        break;
                    }
                }
                SetDialogueFont(text.font);
            }
            started = true;

            DropdownOnValueChanged();
        }



        // Set font of text and save changes in player preferences
        public void DropdownOnValueChanged()
        {
            // Check if the game has started long enough
            if (!started)
                return;

            //Name that is currently selected on the dropDown Menu
            text.text = UIManager.ui_manager.fonts[Maindropdown.value].name;
            text.font = UIManager.ui_manager.fonts[Maindropdown.value];

            SetDialogueFont(text.font);
            PlayerPrefs.Save();
        }


        // Edit this if you want the font to be changeable in different areas than just the dialogue text and speaker text
        public void SetDialogueFont(Font font)
        {
            UIManager.ui_manager.speaker_panel.font = font;
            UIManager.ui_manager.text_panel.font = font;
        }
    }
}