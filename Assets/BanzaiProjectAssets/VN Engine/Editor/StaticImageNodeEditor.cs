using UnityEditor;
using UnityEngine;

namespace VNEngine
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StaticImageNode))]
    public class StaticImageNodeEditor : Editor
    {
        private SerializedProperty image_action;

        // Creating image variables
        private SerializedProperty image_to_place;
        private SerializedProperty place_in_foreground;
        private SerializedProperty size_option;
        private SerializedProperty custom_image_size;
        private SerializedProperty image_coordinates;   // 0,0, being center, -0.5,-0.5 being bottom left, 0.5,0.5 being top right
        private SerializedProperty fade_in_new_image;
        private SerializedProperty fade_in_time;

        // Deleting images variables
        private SerializedProperty image_to_delete;
        private SerializedProperty fade_out_image;
        private SerializedProperty fade_out_time;

        private void OnEnable()
        {
            image_action = serializedObject.FindProperty("image_action");
            place_in_foreground = serializedObject.FindProperty("place_in_foreground");
            image_to_place = serializedObject.FindProperty("image_to_place");
            size_option = serializedObject.FindProperty("size_option");
            custom_image_size = serializedObject.FindProperty("custom_image_size");
            image_coordinates = serializedObject.FindProperty("image_coordinates");
            fade_in_new_image = serializedObject.FindProperty("fade_in_new_image");
            fade_in_time = serializedObject.FindProperty("fade_in_time");
            image_to_delete = serializedObject.FindProperty("image_to_delete");
            fade_out_image = serializedObject.FindProperty("fade_out_image");
            fade_out_time = serializedObject.FindProperty("fade_out_time");
        }

        override public void OnInspectorGUI()
        {
            serializedObject.Update();

            var node = target as StaticImageNode;

            EditorGUILayout.PropertyField(image_action, new GUIContent("Action", "Do you want to create a new static image, or delete one?"));

            switch (node.image_action)
            {
                case StaticImageNode.Image_Action.create_image:
                    EditorGUILayout.PropertyField(image_to_place, new GUIContent("Image to Create", "Select the sprite you want to place on the screen"));
                    EditorGUILayout.PropertyField(place_in_foreground, new GUIContent("Place in foreground", "Static Images in the foreground appear in front of Actors. Otherwise it will appear behind the Actors"));
                    EditorGUILayout.PropertyField(size_option, new GUIContent("Image Size", "How large should the image be? Native size is the actual size of the image file"));

                    if (node.size_option == StaticImageNode.Image_Size.custom_size)
                    {
                        EditorGUILayout.PropertyField(custom_image_size, new GUIContent("Custom Size", "X,Y size of the image, in pixels. Keep in mind your Canvas size's Reference Resolution"));
                    }

                    if (node.size_option == StaticImageNode.Image_Size.custom_size
                        || node.size_option == StaticImageNode.Image_Size.native_size)
                    {
                        EditorGUILayout.HelpBox("0,0 (x,y) is screen center\n-0.5, -0.5 is bottom left\n0.5, 0.5 is top right", MessageType.Info);
                        EditorGUILayout.PropertyField(image_coordinates, new GUIContent("Image Position", "0,0 is screen center, -0.5,-0.5 is bottom left, 0.5,0.5 is top right"));
                    }

                    EditorGUILayout.PropertyField(fade_in_new_image, new GUIContent("Fade in Image", "Fade the image in over time"));

                    if (node.fade_in_new_image)
                        EditorGUILayout.PropertyField(fade_in_time, new GUIContent("Fade in Time (seconds)", "How long it takes to fade in the new image"));

                    break;

                case StaticImageNode.Image_Action.delete_image:
                    EditorGUILayout.HelpBox("Static Images are named after the image file used to create them, and are case sensitive", MessageType.Info);
                    EditorGUILayout.PropertyField(image_to_delete, new GUIContent("Name of Image to delete", "Static Images are named after the image file used to create them, and are case sensitive"));
                    EditorGUILayout.PropertyField(fade_out_image, new GUIContent("Fade out Image", "Fade the image out before deleting it"));

                    if (node.fade_out_image)
                        EditorGUILayout.PropertyField(fade_out_time, new GUIContent("Fade out Time (seconds)", "How long it takes to fade out the image"));

                    break;
            }
            /*
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
            */
            serializedObject.ApplyModifiedProperties();
        }
    }
}