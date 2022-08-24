using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    /*
    This class looks in your LocalizedUI.csv for a given key and puts it in the language given by LocalizationManager.Language
    Attach this to any text element you wish to translate 
    */
    [RequireComponent(typeof(Text))]
    public class LocalizeTextElement : MonoBehaviour
    {
        public string Key;


        void Start()
        {
            LocalizeText();
        }

        public void LocalizeText()
        {
            if (string.IsNullOrEmpty(Key))
            {
                Debug.Log("Key not specified for LocalizeTextElement", this.gameObject);
            }

            string translated_text = UIManager.ui_manager.Get_Localized_UI_Entry(Key);

            if (!string.IsNullOrEmpty(translated_text))
            {
                this.GetComponent<Text>().text = translated_text;
            }
            else
                Debug.Log("LocalizeTextElement could not find key " + Key + " in LocalizedUI.csv", this.gameObject);

            // Add this component to the UIManager's list, so if the language is changed this
            UIManager.ui_manager.Add_Localized_UI_Element_To_List(this);
        }
    }
}