using UnityEngine;
using UnityEditor;

namespace VNEngine
{
    // Give this script a custom UI interface that shows and hides some fields based on the stat_type being used
    [CustomEditor(typeof(AlterStatNode))]
    public class AlterStatNodeEditor : Editor
    {
        override public void OnInspectorGUI()
        {
            var myScript = target as AlterStatNode;

            myScript.stat_type = (Stat_Type)EditorGUILayout.EnumPopup("Stat Type ", myScript.stat_type);
            myScript.stat_name = EditorGUILayout.TextField("Stat Name ", myScript.stat_name);

            // Only show some fields based on stat_type
            switch (myScript.stat_type)
            {
                case Stat_Type.Set_Number:
                    myScript.set_number_to = EditorGUILayout.FloatField("Set number to ", myScript.set_number_to);
                    break;
                case Stat_Type.Modify_Number:
                    myScript.modify_number_amount = EditorGUILayout.FloatField("Add number to ", myScript.modify_number_amount);
                    break;
                case Stat_Type.Set_Boolean:
                    myScript.set_bool_to = EditorGUILayout.Toggle("Set boolean to ", myScript.set_bool_to);
                    break;
                case Stat_Type.Toggle_Boolean:
                    break;
                case Stat_Type.Set_String:
                    myScript.set_string_to = EditorGUILayout.TextField("Set string to ", myScript.set_string_to);
                    break;
            }

            myScript.print_all_stats_to_console = EditorGUILayout.Toggle("Print Stats to console ", myScript.print_all_stats_to_console);
            EditorGUILayout.HelpBox("Stats are used to record values. They can used by If Then Nodes.\n(Right click, VN Engine/If Then)", MessageType.Info);
        }
    }
}