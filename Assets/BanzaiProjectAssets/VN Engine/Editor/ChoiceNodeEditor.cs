using UnityEngine;
using UnityEditor;


namespace VNEngine
{
    // Give this script a custom UI interface used in conjunction with the new Stats ssystem (see AlterStatNode and StatsManager)
    [CustomEditor(typeof(ChoiceNode))]
    public class ChoiceNodeEditor : Editor
    {
        float default_label_width;

        override public void OnInspectorGUI()
        {
            default_label_width = EditorGUIUtility.labelWidth;

            var choices = target as ChoiceNode;

            choices.Name_Of_Choice = EditorGUILayout.TextField("Name of Choice:", choices.Name_Of_Choice);
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 90;
            choices.Number_Of_Choices = Mathf.Abs(Mathf.Min(ChoiceNode.max_number_of_buttons, EditorGUILayout.IntField("# of Choices:", choices.Number_Of_Choices, GUILayout.Width(110))));
            EditorGUIUtility.labelWidth = 50;
            choices.Hide_Dialogue_UI = EditorGUILayout.Toggle("Hide UI:", choices.Hide_Dialogue_UI);
            GUILayout.EndHorizontal();
            for (int x = 0; x < choices.Show_Choice_Was_Selected_Before.Length; x++)
            {
                if (choices.Show_Choice_Was_Selected_Before[x])
                {
                    EditorGUILayout.HelpBox("Show if was previously selected shows a checkmark if the user has clicked that choice before. This uses the Boolean Stats, meaning each 'Button Text' must be UNIQUE (no other Boolean stat can use the same name).", MessageType.Info);
                    break;
                }
            }

            EditorGUIUtility.labelWidth = 150;
            choices.randomize_choices_order = EditorGUILayout.Toggle("Randomize button order:", choices.randomize_choices_order);
            choices.Localize_Choice_Text = EditorGUILayout.Toggle("Localize choice text:", choices.Localize_Choice_Text);
            if (choices.Localize_Choice_Text == true)
            {
                EditorGUILayout.HelpBox("You'll need to set the LocalizedDialogueCSV in the SceneManager. Put the localization Keys you wish to use as the name of the Choice and the button text itself.", MessageType.Info);
            }
            EditorGUIUtility.labelWidth = default_label_width;

            EditorGUIUtility.labelWidth = 180;
            choices.use_image_native_size_for_buttons = EditorGUILayout.Toggle("Use image button native size:", choices.use_image_native_size_for_buttons);

            EditorGUIUtility.labelWidth = 180;
            choices.default_selection = (Select_Default_Choice)EditorGUILayout.EnumPopup("Default Selected Choiced:", choices.default_selection);

            EditorGUILayout.LabelField("");

            // Create editor interface by looping through all of the buttons
            for (int x = 0; x < choices.Number_Of_Choices; x++)
            {
                //int button_number = x + 1;
                EditorGUIUtility.labelWidth = 90;
                choices.Button_Text[x] = EditorGUILayout.TextField("Button " + x + " Text:", choices.Button_Text[x]);
                EditorGUIUtility.labelWidth = 95;
                choices.Has_Requirements[x] = (Choice_Stat_Requirement)EditorGUILayout.EnumPopup("Requires Stats?", choices.Has_Requirements[x]);

                if (choices.Has_Requirements[x] == Choice_Stat_Requirement.Has_Requirements)
                {
                    // Loop through each requirement
                    for (int req = 0; req < 20; req++)
                    {
                        // Stop if we encounter the done
                        if (req > 0 && choices.Logic[x * 20 + req - 1] == Choice_Boolean_Logic.Done)
                        {
                            break;
                        }

                        choices.Requirement_Type[x * 20 + req] = (Choice_Condition)EditorGUILayout.EnumPopup("", choices.Requirement_Type[x * 20 + req]);

                        EditorGUIUtility.labelWidth = default_label_width;
                        GUILayout.BeginHorizontal();
                        switch (choices.Requirement_Type[x * 20 + req])
                        {
                            case Choice_Condition.Float_Stat_Requirement:
                                EditorGUIUtility.labelWidth = 40;
                                choices.Stat_Name[x * 20 + req] = EditorGUILayout.TextField("Stat:", choices.Stat_Name[x * 20 + req]);
                                EditorGUIUtility.labelWidth = 20;
                                choices.Float_Stat_Is[x * 20 + req] = (Float_Stat_Comparator)EditorGUILayout.EnumPopup("is", choices.Float_Stat_Is[x * 20 + req]);
                                choices.Float_Compare_Value[x * 20 + req] = EditorGUILayout.FloatField(choices.Float_Compare_Value[x * 20 + req]);
                                break;

                            case Choice_Condition.Bool_Stat_Requirement:
                                EditorGUIUtility.labelWidth = 40;
                                choices.Stat_Name[x * 20 + req] = EditorGUILayout.TextField("Stat:", choices.Stat_Name[x * 20 + req]);
                                EditorGUIUtility.labelWidth = 20;
                                choices.Bool_Compare_Value[x * 20 + req] = EditorGUILayout.Toggle("is", choices.Bool_Compare_Value[x * 20 + req]);
                                break;


                            case Choice_Condition.String_Stat_Requirement:
                                EditorGUIUtility.labelWidth = 40;
                                choices.Stat_Name[x * 20 + req] = EditorGUILayout.TextField("Stat:", choices.Stat_Name[x * 20 + req]);
                                choices.String_Is[x * 20 + req] = (Result)EditorGUILayout.EnumPopup("", choices.String_Is[x * 20 + req]);
                                choices.String_Compare_Value[x * 20 + req] = EditorGUILayout.TextField(choices.String_Compare_Value[x * 20 + req]);
                                break;


                            case Choice_Condition.Object_Is_Null:
                                EditorGUIUtility.labelWidth = 45;
                                choices.Check_Null_Object[x * 20 + req] = (GameObject)EditorGUILayout.ObjectField("Object ", choices.Check_Null_Object[x * 20 + req], typeof(GameObject), true);
                                choices.Bool_Compare_Value[x * 20 + req] = EditorGUILayout.Toggle("is null: ", choices.Bool_Compare_Value[x * 20 + req]);
                                break;
                        }
                        GUILayout.EndHorizontal();
                        EditorGUIUtility.labelWidth = default_label_width;

                        choices.Logic[x * 20 + req] = (Choice_Boolean_Logic)EditorGUILayout.EnumPopup("", choices.Logic[x * 20 + req]);
                    }

                    // Requirements not met section
                    choices.Requirement_Not_Met_Actions[x] = (Requirement_Not_Met_Action)EditorGUILayout.EnumPopup("Requirement not met?", choices.Requirement_Not_Met_Actions[x]);
                    switch (choices.Requirement_Not_Met_Actions[x])
                    {
                        case Requirement_Not_Met_Action.Disable_Button:
                            choices.Disabled_Text[x] = EditorGUILayout.TextField("Disabled Button Text:", choices.Disabled_Text[x]);
                            break;
                        case Requirement_Not_Met_Action.Hide_Choice:
                            break;
                    }
                }

                EditorGUIUtility.labelWidth = 190;
                choices.Show_Choice_Was_Selected_Before[x] = EditorGUILayout.Toggle("Show if was previously selected:", choices.Show_Choice_Was_Selected_Before[x]);

                //EditorGUIUtility.labelWidth = default_label_width;
                EditorGUILayout.LabelField("");
            }


            EditorGUIUtility.labelWidth = default_label_width;
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}