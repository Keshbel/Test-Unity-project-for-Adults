using UnityEngine;
using UnityEditor;

namespace VNEngine
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DialogueNode))]
    public class DialogueNodeEditor : Editor
    {
        float normal_label_width;
        private SerializedProperty m_darken;
        private SerializedProperty m_front;
        private SerializedProperty m_clear;
        private SerializedProperty m_log;
        private SerializedProperty m_log_category;
        private SerializedProperty m_actor_name_from;
        private SerializedProperty m_actor;
        private SerializedProperty m_textbox;
        private SerializedProperty m_speaker_name_source;
        private SerializedProperty m_text_source;
        private SerializedProperty m_localized_key;
        private SerializedProperty m_talking_beep;
        private SerializedProperty m_fetch_localized_stat_value;

        private void OnEnable()
        {
            m_darken = serializedObject.FindProperty("darken_all_other_characters");
            m_front = serializedObject.FindProperty("bring_speaker_to_front");
            m_clear = serializedObject.FindProperty("clear_text_after");
            m_log = serializedObject.FindProperty("log_text");
            m_log_category = serializedObject.FindProperty("log_category");
            m_actor = serializedObject.FindProperty("actor");
            m_actor_name_from = serializedObject.FindProperty("actor_name_from");
            m_textbox = serializedObject.FindProperty("textbox_title");
            m_text_source = serializedObject.FindProperty("dialogue_from");
            m_speaker_name_source = serializedObject.FindProperty("speaker_name_from");
            m_localized_key = serializedObject.FindProperty("localized_key");
            m_talking_beep = serializedObject.FindProperty("talking_beeps");
            m_fetch_localized_stat_value = serializedObject.FindProperty("fetch_localized_stat_value");
        }

        override public void OnInspectorGUI()
        {
            serializedObject.Update();

            var node = target as DialogueNode;
            normal_label_width = EditorGUIUtility.labelWidth;

            EditorGUILayout.PropertyField(m_actor_name_from, new GUIContent("Actor name from", "Get the name of the actor directly from the editor or from the UI CSV"));
            EditorGUILayout.PropertyField(m_actor, new GUIContent("Actor", "Actors are character images on the scene. The current actor talking, and will have the properties below applied to them. Bring actors onto the scene by using 'EnterActorNodes'."));
            EditorGUILayout.PropertyField(m_speaker_name_source, new GUIContent("Speaker name from", "Get the name of the speaker directly from the editor or from the UI CSV"));

            string speaker_name_tooltip = "Speaker name";
            if (node.speaker_name_from == Dialogue_Source.Localized_Text_From_CSV)
            {
                EditorGUILayout.HelpBox("You must assign a Localized_UI_CSV to the UIManager in the DialogueCanvas/SceneManager to use localized speaker names", MessageType.Info);
                speaker_name_tooltip = "Speaker key";
            }
            else if (node.speaker_name_from == Dialogue_Source.Text_From_String_Stat)
            {
                EditorGUILayout.HelpBox("Enter the name of the String Stat you wish to use below.\nTo create a Stat you can use an AlterStatsNode.\nIf there is no Stat of that name present, the name of Stat will be placed in the speaker title area.", MessageType.Info);
                speaker_name_tooltip = "String Stat name";
            }
            EditorGUILayout.PropertyField(m_textbox, new GUIContent(speaker_name_tooltip, "The name put into the speaker panel above the text panel. It does not have to be the same as the actor name."));
            if (node.speaker_name_from == Dialogue_Source.Text_From_String_Stat)
            {
                EditorGUIUtility.labelWidth = 200;
                EditorGUILayout.PropertyField(m_fetch_localized_stat_value, new GUIContent("Localize value fetched from Stat", "If true, the value fetched from the specified String Stat will be used as a key to look up a localized version of it from your Localized Dialogue CSV. You'll need to add entries with the possible Stat values into your CSV."));
                EditorGUIUtility.labelWidth = normal_label_width;
            }


            GUIStyle g = new GUIStyle();
            g.richText = true;
            EditorGUILayout.LabelField("");
            EditorGUILayout.PropertyField(m_text_source, new GUIContent("Text comes from ", "Text is either entered using the Unity Editor, or retrieved from a localized CSV at runtime"));
            switch (node.dialogue_from)
            {
                case Dialogue_Source.Text_From_Editor:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 300;
                    EditorGUILayout.LabelField("Dialogue  (you can use rich text, ex: <b>Bold</b> becomes");
                    EditorGUIUtility.labelWidth = normal_label_width;
                    EditorGUILayout.LabelField("<b>Bold</b>", g);
                    EditorGUILayout.EndHorizontal();
                    EditorStyles.textField.wordWrap = true;
                    node.text = EditorGUILayout.TextArea(node.text);
                    break;
                case Dialogue_Source.Localized_Text_From_CSV:
                    EditorGUILayout.HelpBox("You must assign a Localized_Dialogue_CSV to the DialogueCanvas/SceneManager to use localized text", MessageType.Info);
                    EditorGUILayout.LabelField("Get text from a CSV you supply. Each row has a key. Enter the key below for the text that you want to retrieve at run-time.");
                    EditorGUILayout.PropertyField(m_localized_key, new GUIContent("Key from CSV", "The key for the row you to use from your Localized Dialogue CSV (located in the VN Scene Manager)"));
                    break;
            }
            EditorGUILayout.LabelField("");
            EditorGUILayout.PropertyField(m_talking_beep, new GUIContent("Talking beeps", "Plays this extremely short audio clip every time a character of text is printed to the text panel"));
            EditorGUILayout.PropertyField(m_darken, new GUIContent("Darken other actors", "Darkens the non-speaking actors on the scene. Same as using 'DarkenActorNode'"));
            EditorGUILayout.PropertyField(m_front, new GUIContent("Bring speaker forward", "Since this is a 2D scene, actors are drawn in a certain order, meaning they can cover one another. This will ensure the speaker will not be covered."));
            EditorGUILayout.PropertyField(m_clear, new GUIContent("Clear text panel after", "Removes all text from the text and speaker panel"));
            
            EditorGUILayout.PropertyField(m_log, new GUIContent("Record text in Log", "Puts the dialogue text into the Text log, which can be accessed via the 'Log' button in the bottom right hand corner."));
            if (node.log_text)
            {
                EditorGUILayout.PropertyField(m_log_category, new GUIContent("Log text under category", "Text in the text log is stored in a dictionary accessed by the string key you type here."));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}