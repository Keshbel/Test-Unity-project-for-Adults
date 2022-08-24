using UnityEditor;
using UnityEngine;


namespace VNEngine
{
    // Give this script a custom UI interface used in conjunction with the new Stats ssystem (see AlterStatNode and StatsManager)
    [CustomEditor(typeof(IfNode))]
    public class IfNodeEditor : Editor
    {
        float default_label_width;

        override public void OnInspectorGUI()
        {
            default_label_width = EditorGUIUtility.labelWidth;

            var if_node = target as IfNode;

            if_node.Number_Of_Conditions = Mathf.Clamp(EditorGUILayout.IntField("Number of Conditions:", if_node.Number_Of_Conditions), 1, IfNode.max_number_of_conditions);
            EditorGUILayout.LabelField("");

            // Loop through every condition
            for (int x = 0; x < if_node.Number_Of_Conditions; x++)
            {
                if_node.Conditions[x] = (Condition)EditorGUILayout.EnumPopup("Condition Type: ", if_node.Conditions[x]);

                GUILayout.BeginHorizontal();
                switch (if_node.Conditions[x])
                {
                    case Condition.Bool_Stat_Requirement:
                        EditorGUIUtility.labelWidth = 40;
                        if_node.Stat_Name[x] = EditorGUILayout.TextField("Stat:", if_node.Stat_Name[x]);
                        if_node.Bool_Compare_Value[x] = EditorGUILayout.Toggle("is", if_node.Bool_Compare_Value[x]);
                        break;


                    case Condition.Float_Stat_Requirement:
                        EditorGUIUtility.labelWidth = 40;
                        if_node.Stat_Name[x] = EditorGUILayout.TextField("Stat:", if_node.Stat_Name[x]);
                        if_node.Float_Stat_Is[x] = (Float_Stat_Comparator)EditorGUILayout.EnumPopup("is", if_node.Float_Stat_Is[x]);
                        if_node.Float_Compare_Value[x] = EditorGUILayout.FloatField(if_node.Float_Compare_Value[x]);
                        break;


                    case Condition.String_Stat_Requirement:
                        EditorGUIUtility.labelWidth = 40;
                        if_node.Stat_Name[x] = EditorGUILayout.TextField("Stat:", if_node.Stat_Name[x]);
                        if_node.String_Is[x] = (Result)EditorGUILayout.EnumPopup("", if_node.String_Is[x]);
                        if_node.String_Compare_Value[x] = EditorGUILayout.TextField(if_node.String_Compare_Value[x]);
                        break;


                    case Condition.Object_Is_Null:
                        EditorGUIUtility.labelWidth = 45;
                        if_node.Check_Null_Object[x] = (GameObject)EditorGUILayout.ObjectField("Object ", if_node.Check_Null_Object[x], typeof(GameObject), true);
                        if_node.Bool_Compare_Value[x] = EditorGUILayout.Toggle("is null: ", if_node.Bool_Compare_Value[x]);
                        break;


                    case Condition.Object_Is_Active:
                        EditorGUIUtility.labelWidth = 70;
                        if_node.Check_Active_Object[x] = (GameObject)EditorGUILayout.ObjectField("Object ", if_node.Check_Active_Object[x], typeof(GameObject), true);
                        if_node.Bool_Compare_Value[x] = EditorGUILayout.Toggle("is active: ", if_node.Bool_Compare_Value[x]);
                        break;
                }
                GUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = default_label_width;

                // Check if we need to put a boolean logic field between these conditions
                if (x < if_node.Number_Of_Conditions - 1)
                {
                    if_node.Logic[x] = (Boolean_Logic)EditorGUILayout.EnumPopup(if_node.Logic[x]);
                }
            }
            EditorGUILayout.LabelField("");

            // Action taken
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 85;
            if_node.Is_Condition_Met = (Condition_Is)EditorGUILayout.EnumPopup("If condition is ", if_node.Is_Condition_Met);
            if_node.Action = (Requirement_Met_Action)EditorGUILayout.EnumPopup("", if_node.Action);
            EditorGUIUtility.labelWidth = default_label_width;
            GUILayout.EndHorizontal();
            switch (if_node.Action)
            {
                case Requirement_Met_Action.Change_Conversation:
                    if_node.Conversation_To_Switch_To = (ConversationManager)EditorGUILayout.ObjectField("Start Conversation: ", if_node.Conversation_To_Switch_To, typeof(ConversationManager), true);
                    break;
                case Requirement_Met_Action.Jump_to_Middle_of_Conversation:
                    if_node.Node_To_Switch_To = (Node)EditorGUILayout.ObjectField("Jump to Node: ", if_node.Node_To_Switch_To, typeof(Node), true);
                    break;
                case Requirement_Met_Action.Custom_Events:
                    // Show the Button.OnClicked Events
                    base.OnInspectorGUI();
                    if_node.Continue_Conversation = EditorGUILayout.Toggle("Continue Conversation After? ", if_node.Continue_Conversation);
                    break;
            }
        }
    }
}