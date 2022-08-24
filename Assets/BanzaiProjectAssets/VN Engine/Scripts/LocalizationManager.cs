using UnityEngine;
using System.Collections;

namespace VNEngine
{
    // Contains information about our language and supported languages
    public static class LocalizationManager
    {
        // Set these to be the same languages used in the LocalizedUI.csv
        public static string[] Supported_Languages = { "English", "French", "Spanish" };

        // Must be set to our current language so our UI can be displayed properly
        public static string Language = Supported_Languages[0];


        public static void Set_Language(string language)
        {
            Debug.Log("Setting language: " + language);
            LocalizationManager.Language = language;

            // Save language in player preferences
            PlayerPrefs.SetString("Language", language);
            PlayerPrefs.Save();
        }
    }
}